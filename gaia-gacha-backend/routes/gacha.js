import express from 'express';

export default function (supabase) {
  const router = express.Router();

  // --- GACHA CONFIGURATION ---
  const GACHA_COST = 10;

  // --- THE GACHA ROUTE ---
  router.post('/pull', async (req, res) => {
    const { userId } = req.body; 

    try {
      // 1. Fetch Player Data
      const { data: player, error: playerError } = await supabase
        .from('profiles')
        .select('coins')
        .eq('id', userId)
        .single();

      if (playerError || !player) {
        console.error("Fetch Player Error:", playerError?.message);
        return res.status(404).json({ error: "Player not found" });
      }

      // 2. Check Currency
      if (player.coins < GACHA_COST) {
        return res.status(400).json({ error: "Not enough coins!" });
      }

      // 3. Logic: Weighted Random Selection
      const { data: items, error: itemsError } = await supabase
        .from('items')
        .select('id, name, rarity, weight');

      if (itemsError || !items || items.length === 0) {
        console.error("Fetch Items Error:", itemsError?.message);
        return res.status(500).json({ error: "Failed to load items" });
      }

      const roll = Math.random() * 100;
      let selectedItem = items[0];
      let cumulativeWeight = 0;

      for (const item of items) {
        cumulativeWeight += item.weight;
        if (roll < cumulativeWeight) {
          selectedItem = item;
          break;
        }
      }

      const newBalance = player.coins - GACHA_COST;

      // 4. Update Database 
      // Subtract coins from 'profiles' table
      const { error: deductError } = await supabase
        .from('profiles')
        .update({ coins: newBalance })
        .eq('id', userId);

      if (deductError) {
        console.error("Deduct Coins Database Error:", deductError.message);
        return res.status(500).json({ error: "Failed to process coin deduction" });
      }

      // Add item to 'inventory' table
      const { error: insertError } = await supabase
        .from('inventory')
        .insert([{
          user_id: userId,
          item_id: selectedItem.id
        }]);

      if (insertError) {
        // If RLS or policy errors happen, this will print it directly to the terminal screen
        console.error("Inventory Insertion Database Error:", insertError.message);
        return res.status(500).json({ error: `Failed to secure item in inventory: ${insertError.message}` });
      }

      // 6. Send Result back to Unity
      return res.json({
        message: `You found a ${selectedItem.name}!`,
        item: { id: selectedItem.id, name: selectedItem.name, rarity: selectedItem.rarity },
        newBalance: newBalance
      });

    } catch (err) {
      console.error("System Route Exception Caught:", err);
      return res.status(500).json({ error: "Server Error" });
    }
  });

  return router;
}