// gacha.test.js: unit tests for gacha.js's exported selectWeightedRandomItem
// function, run in isolation from Express and Supabase.

import { describe, it, expect, vi, afterEach } from 'vitest';
import { selectWeightedRandomItem } from '../../routes/gacha.js';

describe('selectWeightedRandomItem', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('returns undefined for an empty pool', () => {
    expect(selectWeightedRandomItem([])).toBeUndefined();
  });

  it('returns the only item when the pool has a single item', () => {
    const items = [{ id: 1, weight: 50 }];
    expect(selectWeightedRandomItem(items)).toEqual(items[0]);
  });

  it('never selects a zero-weight item', () => {
    const zeroWeightItem = { id: 1, weight: 0 };
    const items = [zeroWeightItem, { id: 2, weight: 100 }];

    // Math.random() * 100 spans [0, 100). Sampling across that range confirms the
    // zero-weight item's cumulative threshold (0) can never be reached.
    for (const roll of [0, 0.001, 0.25, 0.5, 0.75, 0.999]) {
      vi.spyOn(Math, 'random').mockReturnValue(roll);
      expect(selectWeightedRandomItem(items)).toEqual(items[1]);
    }
  });

  it('selects higher-weighted items more often over many trials', () => {
    const items = [
      { id: 'common', weight: 90 },
      { id: 'rare', weight: 10 }
    ];

    const counts = { common: 0, rare: 0 };
    for (let i = 0; i < 1000; i++) {
      counts[selectWeightedRandomItem(items).id]++;
    }

    expect(counts.common).toBeGreaterThan(counts.rare);
  });
});
