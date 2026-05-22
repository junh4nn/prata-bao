import express from 'express';

export default function (supabase) {
  const router = express.Router();

  // --- GACHA CONFIGURATION ---
  const GACHA_COST = 10;
  const ITEMS = [
    { id: 1, name: 'Mangrove Seed', rarity: 'Common', weight: 70 },
    { id: 2, name: 'Coral Fragment', rarity: 'Rare', weight: 25 },
    { id: 3, name: 'Giant Sea Turtle Shell', rarity: 'Legendary', weight: 5 }
  ];

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

      if (playerError || !player) return res.status(404).json({ error: "Player not found" });

      // 2. Check Currency
      if (player.coins < GACHA_COST) {
        return res.status(400).json({ error: "Not enough coins!" });
      }

      // 3. Logic: Weighted Random Selection
      const roll = Math.random() * 100;
      let selectedItem = ITEMS[0];
      let cumulativeWeight = 0;

      for (const item of ITEMS) {
        cumulativeWeight += item.weight;
        if (roll < cumulativeWeight) {
          selectedItem = item;
          break;
        }
      }

      // 4. Update Database (Subtract coins & Add item)
      await supabase
        .from('profiles')
        .update({ coins: player.coins - GACHA_COST })
        .eq('id', userId);

      await supabase
        .from('inventory')
        .insert([{ 
          user_id: userId, 
          item_name: selectedItem.name,
          rarity: selectedItem.rarity
        }]);

      // 5. Send Result back to Unity
      return res.json({
        message: `You found a ${selectedItem.name}!`,
        item: selectedItem,
        newBalance: player.coins - GACHA_COST
      });

    } catch (err) {
      return res.status(500).json({ error: "Server Error" });
    }
  });

  return router;
}