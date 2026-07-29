// inventory.test.js: integration tests for inventory.js's GET / route, against a
// real local Supabase instance. Most cases use a stub auth middleware; one uses
// the real app.js wiring with a real token.

import { describe, it, expect, beforeAll, beforeEach } from 'vitest';
import request from 'supertest';
import express from 'express';

import inventoryRouter from '../../routes/inventory.js';
import realApp from '../../app.js';
import { supabase } from './helpers/supabaseClients.js';
import { createStubAuth } from './helpers/stubAuth.js';
import { getOrCreateTestUser } from './helpers/testUser.js';

const MANGROVE_SEED_ID = 1;
const CORAL_FRAGMENT_ID = 2;

let testUserId;
let testUserToken;
let testApp;

beforeAll(async () => {
  const testUser = await getOrCreateTestUser();
  testUserId = testUser.userId;
  testUserToken = testUser.token;

  testApp = express();
  testApp.use(express.json());
  testApp.use('/api/inventory', inventoryRouter(supabase, createStubAuth(testUserId)));
});

describe('GET /api/inventory', () => {
  beforeEach(async () => {
    await supabase.from('inventory').delete().eq('user_id', testUserId);
  });

  it('returns { items: [] } for an empty inventory', async () => {
    const res = await request(testApp).get('/api/inventory');

    expect(res.status).toBe(200);
    expect(res.body.items).toEqual([]);
  });

  it('groups multiple copies of the same item with the correct count', async () => {
    await supabase.from('inventory').insert([
      { user_id: testUserId, item_id: MANGROVE_SEED_ID },
      { user_id: testUserId, item_id: MANGROVE_SEED_ID },
      { user_id: testUserId, item_id: MANGROVE_SEED_ID },
    ]);

    const res = await request(testApp).get('/api/inventory');

    expect(res.status).toBe(200);
    expect(res.body.items).toHaveLength(1);
    expect(res.body.items[0]).toMatchObject({
      itemId: MANGROVE_SEED_ID,
      name: 'Mangrove Seed',
      count: 3,
    });
  });

  it('groups multiple distinct items separately, each with the correct firstObtainedAt', async () => {
    await supabase.from('inventory').insert([
      { user_id: testUserId, item_id: MANGROVE_SEED_ID, created_at: '2026-03-01T00:00:00Z' },
      { user_id: testUserId, item_id: MANGROVE_SEED_ID, created_at: '2026-01-01T00:00:00Z' },
      { user_id: testUserId, item_id: CORAL_FRAGMENT_ID, created_at: '2026-02-01T00:00:00Z' },
    ]);

    const res = await request(testApp).get('/api/inventory');

    expect(res.status).toBe(200);
    expect(res.body.items).toHaveLength(2);

    const mangrove = res.body.items.find((item) => item.itemId === MANGROVE_SEED_ID);
    expect(mangrove).toMatchObject({ count: 2 });
    expect(new Date(mangrove.firstObtainedAt).toISOString()).toBe('2026-01-01T00:00:00.000Z');

    const coral = res.body.items.find((item) => item.itemId === CORAL_FRAGMENT_ID);
    expect(coral).toMatchObject({ count: 1 });
  });

  it('works through the real app.js wiring with a real token', async () => {
    const res = await request(realApp)
      .get('/api/inventory')
      .set('Authorization', `Bearer ${testUserToken}`);

    expect(res.status).toBe(200);
    expect(res.body.items).toEqual([]);
  });
});
