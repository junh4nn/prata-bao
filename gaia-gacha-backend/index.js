import dotenv from 'dotenv';
import express from 'express';
import { createClient } from '@supabase/supabase-js';
import cors from 'cors';

import gachaRouter from './routes/gacha.js';
import authRouter from './routes/auth.js';
import inventoryRouter from './routes/inventory.js';

dotenv.config();

const app = express();
app.use(cors());
app.use(express.json());

const supabase = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_SERVICE_ROLE_KEY
);

// Separate client for verifying player logins. signInWithPassword() attaches the
// signed-in player's session to whichever client it's called on, so it must never
// run on the service-role `supabase` client above — that would silently swap every
// later admin-level query (gacha pulls, inventory) onto that player's own permissions.
const supabaseAuth = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_ANON_KEY
);

// =====================================
// MOUNT ROUTERS
// =====================================
app.use('/api/gacha', gachaRouter(supabase)); // Mounts gacha routes under /api/gacha
app.use('/api/auth', authRouter(supabase, supabaseAuth)); // Mounts auth routes under /api/auth
app.use('/api/inventory', inventoryRouter(supabase)); // Mounts inventory routes under /api/inventory

// Optional base fallback route for checking server status in a browser
app.get('/', (req, res) => {
  res.send('GaiaGacha secure backend API running.');
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));