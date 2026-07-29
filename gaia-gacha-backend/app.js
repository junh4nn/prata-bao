// app.js: the application module. Builds and exports the configured Express app
// (Supabase clients, JWT verification, and the gacha/auth/inventory/quiz routers
// under /api/*), but never starts it. index.js is its closest collaborator: it loads
// environment variables and calls app.listen() on the app this file exports.

import express from 'express';
import { createClient } from '@supabase/supabase-js';
import cors from 'cors';

import gachaRouter from './routes/gacha.js';
import authRouter from './routes/auth.js';
import inventoryRouter from './routes/inventory.js';
import quizRouter from './routes/quiz.js';
import createAuthMiddleware from './middleware/requireAuth.js';

const app = express();
const allowedOrigin = process.env.ALLOWED_ORIGIN || 'http://localhost:3000';
app.use(cors({ origin: allowedOrigin }));
app.use(express.json());

const supabase = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_SERVICE_ROLE_KEY
);

// Separate client for verifying player logins. signInWithPassword() attaches the
// signed-in player's session to whichever client it's called on, so it must never
// run on the service-role `supabase` client above. Doing so would silently swap every
// later admin-level query (gacha pulls, inventory) onto that player's own permissions.
const supabaseAuth = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_ANON_KEY
);

const requireAuth = createAuthMiddleware(supabaseAuth);

// --- Mount Routers ---
app.use('/api/gacha', gachaRouter(supabase, requireAuth));
app.use('/api/auth', authRouter(supabase, supabaseAuth));
app.use('/api/inventory', inventoryRouter(supabase, requireAuth));
app.use('/api/quiz', quizRouter(supabase, requireAuth));

// --- Status Routes ---
// Optional base fallback route for checking server status in a browser
app.get('/', (req, res) => {
  res.send('GaiaGacha secure backend API running.');
});

// Health check for Fly's bluegreen deploy strategy, confirms the new machine is ready before traffic switches over
app.get('/health', (req, res) => {
  res.sendStatus(200);
});

export default app;
