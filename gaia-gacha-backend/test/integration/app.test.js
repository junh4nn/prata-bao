// app.test.js: integration test for app.js's own /health route, the smoke test
// Fly's bluegreen deploy strategy relies on to confirm a new machine is ready
// before traffic switches over. No stub auth or test user needed here, since
// /health takes no auth and touches no database.

import { describe, it, expect } from 'vitest';
import request from 'supertest';

import app from '../../app.js';

describe('GET /health', () => {
  it('returns 200', async () => {
    const res = await request(app).get('/health');
    expect(res.status).toBe(200);
  });
});
