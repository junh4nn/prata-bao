// quiz.test.js: unit tests for quiz.js's exported shuffle function, run in isolation
// from Express and Supabase.

import { describe, it, expect } from 'vitest';
import { shuffle } from '../../routes/quiz.js';

describe('shuffle', () => {
  it('returns an array of the same length as the input', () => {
    const input = [1, 2, 3, 4, 5];
    expect(shuffle(input)).toHaveLength(input.length);
  });

  it('returns the same elements as the input, just reordered', () => {
    const input = [1, 2, 3, 4, 5];
    expect([...shuffle(input)].sort()).toEqual([...input].sort());
  });

  it('does not mutate the input array', () => {
    const input = [1, 2, 3, 4, 5];
    const original = [...input];
    shuffle(input);
    expect(input).toEqual(original);
  });

  it('does not always return the original order', () => {
    const input = [1, 2, 3, 4, 5, 6, 7, 8];
    const results = Array.from({ length: 100 }, () => shuffle(input));
    const everyResultMatchesOriginal = results.every(result =>
      result.every((value, index) => value === input[index])
    );
    expect(everyResultMatchesOriginal).toBe(false);
  });
});
