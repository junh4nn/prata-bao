// supabaseClients.js: real Supabase clients for integration tests, built from the
// env vars loaded via --env-file=.env.test. Used both to build routers under test
// and to verify resulting DB state directly after a request.

import { createClient } from '@supabase/supabase-js';

export const supabase = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_SERVICE_ROLE_KEY
);

export const supabaseAuth = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_ANON_KEY
);
