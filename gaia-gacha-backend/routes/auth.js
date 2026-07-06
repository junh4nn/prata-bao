// auth.js: handles player registration and login against Supabase, issuing the
// session JWT that AuthManager.cs stores and every other route now requires.

import express from 'express';

export default function (supabase, supabaseAuth) {
  const router = express.Router();

  // --- Register a New Player ---
  router.post('/register', async (req, res) => {
    const { email, password } = req.body;
    console.log(`[register] email: ${email}`);

    if (!email || !password) {
      return res.status(400).json({ error: 'Email and password are required.' });
    }

    try {
      const { data, error } = await supabase.auth.admin.createUser({
        email: email,
        password: password,
        email_confirm: true
      });

      if (error) throw error;

      return res.status(201).json({ message: 'Account created successfully!' });

    } catch (error) {
      console.error(`[register] Supabase error: ${error.message}`);
      return res.status(400).json({ error: error.message });
    }
  });

  // --- Log In a Player ---
  router.post('/login', async (req, res) => {
    const { email, password } = req.body;

    try {
      // Must run on the anon-key client, not the service-role `supabase` client:
      // signInWithPassword() attaches the resulting session to whichever client it's called on.
      const { data, error } = await supabaseAuth.auth.signInWithPassword({
        email: email,
        password: password
      });

      if (error) throw error;

      const { data: profile } = await supabase
        .from('profiles')
        .select('coins')
        .eq('id', data.user.id)
        .single();

      return res.status(200).json({
        message: 'Login successful!',
        token: data.session.access_token,
        userId: data.user.id,
        coins: profile?.coins ?? 0
      });

    } catch (error) {
      return res.status(400).json({ error: error.message });
    }
  });

  return router;
}