// testUser.js: idempotently creates (or signs into) a single throwaway test user for
// integration tests. Reused by both stub-auth tests (just need the user's id, so a
// real profiles row exists via the on_auth_user_created trigger) and real-wiring
// tests (need an actual token from local Supabase Auth).

import { supabase, supabaseAuth } from './supabaseClients.js';

export const TEST_EMAIL = 'integration-test-user@example.com';
export const TEST_PASSWORD = 'test-password-123';

export async function getOrCreateTestUser() {
  const { error: createError } = await supabase.auth.admin.createUser({
    email: TEST_EMAIL,
    password: TEST_PASSWORD,
    email_confirm: true,
  });

  if (createError && createError.code !== 'email_exists') {
    throw createError;
  }

  const { data, error: signInError } = await supabaseAuth.auth.signInWithPassword({
    email: TEST_EMAIL,
    password: TEST_PASSWORD,
  });

  if (signInError) throw signInError;

  return { userId: data.user.id, token: data.session.access_token };
}
