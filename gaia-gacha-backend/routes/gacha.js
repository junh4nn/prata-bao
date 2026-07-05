// gacha.js: handles the coin-spend gacha pull. Verifies the caller via requireAuth,
// picks a weighted-random item, and updates the player's coins and inventory in Supabase.

import express from 'express';

export default function (supabase, requireAuth) {
  const router = express.Router();

  // --- Gacha Configuration ---
  const GACHA_COST = 10;

  // --- Gacha Pull Route ---
  router.post('/pull', requireAuth, async (req, res) => {
    const userId = req.userId;

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

      // 3. Select a Weighted Random Item
      const { data: items, error: itemsError } = await supabase
        .from('items')
        .select('id, name, rarity, weight, type');

      if (itemsError || !items || items.length === 0) {
        console.error("Fetch Items Error:", JSON.stringify(itemsError, null, 2));
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

      // 4. Deduct the Gacha Cost
      const { error: deductError } = await supabase
        .from('profiles')
        .update({ coins: newBalance })
        .eq('id', userId);

      if (deductError) {
        console.error("Deduct Coins Database Error:", deductError.message);
        return res.status(500).json({ error: "Failed to process coin deduction" });
      }

      // 5. Add the Item to Inventory
      const { error: insertError } = await supabase
        .from('inventory')
        .insert([{
          user_id: userId,
          item_id: selectedItem.id
        }]);

      if (insertError) {
        console.error("Inventory Insertion Database Error:", insertError.message);
        return res.status(500).json({ error: 'Failed to secure item in inventory' });
      }

      // 6. Send Result back to Unity
      return res.json({
        message: `You found a ${selectedItem.name}!`,
        item: { id: selectedItem.id, name: selectedItem.name, rarity: selectedItem.rarity, type: selectedItem.type },
        newBalance: newBalance
      });

    } catch (err) {
      console.error("System Route Exception Caught:", err);
      return res.status(500).json({ error: "Server Error" });
    }
  });

  return router;
}