// requireAuth.test.js: unit tests for requireAuth.js's JWT-checking logic, run with a
// fake supabaseAuth client so no real network call ever happens.

import { describe, it, expect, vi } from 'vitest';
import createAuthMiddleware from '../../middleware/requireAuth.js';

function createMockRes() {
  return {
    status: vi.fn().mockReturnThis(),
    json: vi.fn()
  };
}

describe('createAuthMiddleware', () => {
  it('responds 401 when the Authorization header is missing', async () => {
    const fakeSupabaseAuth = { auth: { getUser: vi.fn() } };
    const requireAuth = createAuthMiddleware(fakeSupabaseAuth);

    const req = { headers: {} };
    const res = createMockRes();
    const next = vi.fn();

    await requireAuth(req, res, next);

    expect(res.status).toHaveBeenCalledWith(401);
    expect(res.json).toHaveBeenCalledWith({ error: 'Missing or invalid Authorization header' });
    expect(next).not.toHaveBeenCalled();
    expect(fakeSupabaseAuth.auth.getUser).not.toHaveBeenCalled();
  });

  it('responds 401 when the token is invalid or expired', async () => {
    const fakeSupabaseAuth = {
      auth: {
        getUser: vi.fn().mockResolvedValue({ data: null, error: { message: 'invalid token' } })
      }
    };
    const requireAuth = createAuthMiddleware(fakeSupabaseAuth);

    const req = { headers: { authorization: 'Bearer bad-token' } };
    const res = createMockRes();
    const next = vi.fn();

    await requireAuth(req, res, next);

    expect(res.status).toHaveBeenCalledWith(401);
    expect(res.json).toHaveBeenCalledWith({ error: 'Invalid or expired session' });
    expect(next).not.toHaveBeenCalled();
  });

  it('sets req.userId and calls next() on a valid token', async () => {
    const fakeSupabaseAuth = {
      auth: {
        getUser: vi.fn().mockResolvedValue({ data: { user: { id: 'user-123' } }, error: null })
      }
    };
    const requireAuth = createAuthMiddleware(fakeSupabaseAuth);

    const req = { headers: { authorization: 'Bearer good-token' } };
    const res = createMockRes();
    const next = vi.fn();

    await requireAuth(req, res, next);

    expect(req.userId).toBe('user-123');
    expect(next).toHaveBeenCalledOnce();
    expect(res.status).not.toHaveBeenCalled();
  });
});
