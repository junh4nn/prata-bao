// inventory.js: returns the authenticated player's owned items, grouped and counted
// from the raw per-pull inventory rows Supabase stores.

import express from 'express';

// Groups raw per-pull inventory rows into one entry per item, counting duplicates and
// tracking the earliest obtained date. Exported at module level so that unit tests can
// exercise it directly (empty inventory, duplicates, out-of-order rows).
export function groupInventoryByItem(rows) {
  const grouped = {};
  for (const row of rows) {
    const g = grouped[row.item_id] ??= {
      itemId: row.item_id, name: row.items.name, rarity: row.items.rarity, type: row.items.type,
      count: 0, firstObtainedAt: row.created_at
    };
    g.count += 1;
    if (row.created_at < g.firstObtainedAt) g.firstObtainedAt = row.created_at;
  }
  return Object.values(grouped);
}

export default function (supabase, requireAuth) {
  const router = express.Router();

  router.get('/', requireAuth, async (req, res) => {
    // 1. Fetch Raw Inventory Rows (one row per copy owned)
    const { data, error } = await supabase
      .from('inventory')
      .select('item_id, created_at, items(name, rarity, type)')
      .eq('user_id', req.userId);

    if (error) return res.status(500).json({ error: 'Failed to load inventory' });

    // 2. Send Grouped Inventory back to Unity
    res.json({ items: groupInventoryByItem(data) });
  });

  return router;
}
