// inventory.test.js: unit tests for inventory.js's exported groupInventoryByItem
// function, run in isolation from Express and Supabase.

import { describe, it, expect } from 'vitest';
import { groupInventoryByItem } from '../../routes/inventory.js';

describe('groupInventoryByItem', () => {
  it('returns an empty array for an empty inventory', () => {
    expect(groupInventoryByItem([])).toEqual([]);
  });

  it('counts duplicate copies of the same item', () => {
    const rows = [
      { item_id: 1, created_at: '2026-01-01', items: { name: 'Sword', rarity: 'common', type: 'weapon' } },
      { item_id: 1, created_at: '2026-01-02', items: { name: 'Sword', rarity: 'common', type: 'weapon' } },
      { item_id: 1, created_at: '2026-01-03', items: { name: 'Sword', rarity: 'common', type: 'weapon' } }
    ];

    const result = groupInventoryByItem(rows);

    expect(result).toHaveLength(1);
    expect(result[0]).toMatchObject({ itemId: 1, name: 'Sword', count: 3 });
  });

  it('groups distinct items separately', () => {
    const rows = [
      { item_id: 1, created_at: '2026-01-01', items: { name: 'Sword', rarity: 'common', type: 'weapon' } },
      { item_id: 2, created_at: '2026-01-01', items: { name: 'Shield', rarity: 'rare', type: 'armour' } }
    ];

    const result = groupInventoryByItem(rows);

    expect(result).toHaveLength(2);
    expect(result.find(item => item.itemId === 1)).toMatchObject({ name: 'Sword', count: 1 });
    expect(result.find(item => item.itemId === 2)).toMatchObject({ name: 'Shield', count: 1 });
  });

  it('picks the earliest firstObtainedAt even when rows arrive out of order', () => {
    const rows = [
      { item_id: 1, created_at: '2026-03-01', items: { name: 'Sword', rarity: 'common', type: 'weapon' } },
      { item_id: 1, created_at: '2026-01-01', items: { name: 'Sword', rarity: 'common', type: 'weapon' } },
      { item_id: 1, created_at: '2026-02-01', items: { name: 'Sword', rarity: 'common', type: 'weapon' } }
    ];

    const result = groupInventoryByItem(rows);

    expect(result[0].firstObtainedAt).toBe('2026-01-01');
  });
});
