import dotenv from 'dotenv';
import express from 'express';
import { createClient } from '@supabase/supabase-js';
import cors from 'cors';

import gachaRouter from './routes/gacha.js';
import authRouter from './routes/auth.js';

dotenv.config();

const app = express();
app.use(cors());
app.use(express.json());

const supabase = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_SERVICE_ROLE_KEY
);

// =====================================
// MOUNT ROUTERS
// =====================================
app.use('/api/gacha', gachaRouter(supabase)); // Mounts gacha routes under /api/gacha
app.use('/api/auth', authRouter(supabase)); // Mounts auth routes under /api/auth

// Optional base fallback route for checking server status in a browser
app.get('/', (req, res) => {
  res.send('GaiaGacha secure backend API running.');
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));