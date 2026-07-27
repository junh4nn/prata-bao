// reset-local-db.js: resets the local Supabase database to a known state before
// integration tests run. Wired in as pretest:integration, so npm runs this
// automatically before test:integration executes.

import { execSync } from 'node:child_process';

if (!/127\.0\.0\.1|localhost/.test(process.env.SUPABASE_URL ?? '')) {
  throw new Error(
    `Refusing to reset the database: SUPABASE_URL does not look local (${process.env.SUPABASE_URL}). ` +
    'This guard exists to prevent a missing or misconfigured .env.test from resetting staging/production.'
  );
}

execSync('supabase db reset', { stdio: 'inherit' });
