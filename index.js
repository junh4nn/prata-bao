require('dotenv').config();
const express = require('express');
const { createClient } = require('@supabase/supabase-js');
const cors = require('cors');

const app = express();
app.use(cors());
app.use(express.json());

const supabase = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_SERVICE_ROLE_KEY
);

// --- GACHA CONFIGURATION ---
const GACHA_COST = 10;
const ITEMS = [
  { id: 1, name: 'Mangrove Seed', rarity: 'Common', weight: 70 },
  { id: 2, name: 'Coral Fragment', rarity: 'Rare', weight: 25 },
  { id: 3, name: 'Giant Sea Turtle Shell', rarity: 'Legendary', weight: 5 }
];

// --- THE GACHA ROUTE ---
app.post('/pull', async (req, res) => {
  const { userId } = req.body; // Unity will send the Player's ID

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
    // We update the coins first
    await supabase
      .from('profiles')
      .update({ coins: player.coins - GACHA_COST })
      .eq('id', userId);

    // Then we record the new item in an 'inventory' table
    const { error: invError } = await supabase
      .from('inventory')
      .insert([{ user_id: userId, item_name: selectedItem.name }]);

    // 5. Send Result back to Unity
    res.json({
      message: `You found a ${selectedItem.name}!`,
      item: selectedItem,
      newBalance: player.coins - GACHA_COST
    });

  } catch (err) {
    res.status(500).json({ error: "Server Error" });
  }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));