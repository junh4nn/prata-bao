// gacha.test.js: integration tests for gacha.js's POST /pull route, against a real
// local Supabase instance. Most cases use a stub auth middleware to test route
// logic in isolation; one uses the real app.js wiring with a real token.

import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import request from 'supertest';
import express from 'express';

import gachaRouter from '../../routes/gacha.js';
import realApp from '../../app.js';
import { supabase } from './helpers/supabaseClients.js';
import { createStubAuth } from './helpers/stubAuth.js';
import { getOrCreateTestUser } from './helpers/testUser.js';

const NONEXISTENT_USER_ID = '00000000-0000-0000-0000-000000000000';
const STARTING_COINS = 250;
const GACHA_COST = 10;

let testUserId;
let testUserToken;
let testApp;

beforeAll(async () => {
  const testUser = await getOrCreateTestUser();
  testUserId = testUser.userId;
  testUserToken = testUser.token;

  testApp = express();
  testApp.use(express.json());
  testApp.use('/api/gacha', gachaRouter(supabase, createStubAuth(testUserId)));
});

describe('POST /api/gacha/pull', () => {
  const notFoundApp = express();
  notFoundApp.use(express.json());
  notFoundApp.use('/api/gacha', gachaRouter(supabase, createStubAuth(NONEXISTENT_USER_ID)));

  // Shared by every test that performs a real pull (mutates coins/inventory).
  afterEach(async () => {
    await supabase.from('profiles').update({ coins: STARTING_COINS }).eq('id', testUserId);
    await supabase.from('inventory').delete().eq('user_id', testUserId);
  });

  it('succeeds with enough coins', async () => {
    const res = await request(testApp).post('/api/gacha/pull');

    expect(res.status).toBe(200);
    expect(res.body.newBalance).toBe(STARTING_COINS - GACHA_COST);
    expect(res.body.item).toHaveProperty('id');

    const { data: inventoryRows } = await supabase
      .from('inventory')
      .select('*')
      .eq('user_id', testUserId);
    expect(inventoryRows).toHaveLength(1);
  });

  it('returns 404 when the player is not found', async () => {
    const res = await request(notFoundApp).post('/api/gacha/pull');
    expect(res.status).toBe(404);
  });

  it('returns 400 when coins are below GACHA_COST', async () => {
    await supabase.from('profiles').update({ coins: 5 }).eq('id', testUserId);

    const res = await request(testApp).post('/api/gacha/pull');
    expect(res.status).toBe(400);
  });

  it('works through the real app.js wiring with a real token', async () => {
    const res = await request(realApp)
      .post('/api/gacha/pull')
      .set('Authorization', `Bearer ${testUserToken}`);

    expect(res.status).toBe(200);
    expect(res.body.newBalance).toBe(STARTING_COINS - GACHA_COST);
  });
});
