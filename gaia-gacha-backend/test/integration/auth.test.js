// auth.test.js: integration tests for auth.js's POST /register and POST /login
// routes, against a real local Supabase instance. No stub auth is needed here —
// these routes produce authentication rather than consume it, so they're built
// with the real supabase/supabaseAuth clients directly.

import { describe, it, expect, beforeAll } from 'vitest';
import request from 'supertest';
import express from 'express';

import authRouter from '../../routes/auth.js';
import { supabase, supabaseAuth } from './helpers/supabaseClients.js';
import { getOrCreateTestUser, TEST_EMAIL, TEST_PASSWORD } from './helpers/testUser.js';

let testApp;

beforeAll(async () => {
  await getOrCreateTestUser();

  testApp = express();
  testApp.use(express.json());
  testApp.use('/api/auth', authRouter(supabase, supabaseAuth));
});

describe('POST /api/auth/register', () => {
  it('succeeds with a new email', async () => {
    const newEmail = `register-success-${Date.now()}@example.com`;

    try {
      const res = await request(testApp)
        .post('/api/auth/register')
        .send({ email: newEmail, password: 'some-password-123' });

      expect(res.status).toBe(201);
    } finally {
      const { data } = await supabase.auth.admin.listUsers();
      const created = data.users.find((user) => user.email === newEmail);
      if (created) await supabase.auth.admin.deleteUser(created.id);
    }
  });

  it('returns 400 for a duplicate email', async () => {
    const res = await request(testApp)
      .post('/api/auth/register')
      .send({ email: TEST_EMAIL, password: TEST_PASSWORD });

    expect(res.status).toBe(400);
  });
});

describe('POST /api/auth/login', () => {
  it('succeeds with valid credentials', async () => {
    const res = await request(testApp)
      .post('/api/auth/login')
      .send({ email: TEST_EMAIL, password: TEST_PASSWORD });

    expect(res.status).toBe(200);
    expect(typeof res.body.token).toBe('string');
    expect(typeof res.body.userId).toBe('string');
    expect(typeof res.body.coins).toBe('number');
  });

  it('returns 400 for the wrong password', async () => {
    const res = await request(testApp)
      .post('/api/auth/login')
      .send({ email: TEST_EMAIL, password: 'definitely-the-wrong-password' });

    expect(res.status).toBe(400);
  });
});
