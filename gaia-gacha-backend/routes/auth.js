import express from 'express';

export default function (supabase) {
  const router = express.Router();

  // =====================================
  // ROUTE: REGISTER A NEW PLAYER
  // URL: http://localhost:3000/api/auth/register
  // =====================================
  router.post('/register', async (req, res) => {
    const { email, password } = req.body;
    
    // Simple validation check
    if (!email || !password) {
      return res.status(400).json({ error: 'Email and password are required.' });
    }

    try {
      // Instruct Supabase to securely create the user account
      const { data, error } = await supabase.auth.admin.createUser({
        email: email,
        password: password,
        email_confirm: true // Automatically confirms email for smooth local testing
      });

      if (error) throw error;

      return res.status(201).json({ 
        message: 'Account created successfully!' 
      });

    } catch (error) {
      // If Supabase says the email is taken or password is too weak, catch it here
      return res.status(400).json({ error: error.message });
    }
  });

  // =====================================
  // ROUTE: LOGIN A PLAYER
  // URL: http://localhost:3000/api/auth/login
  // =====================================
  router.post('/login', async (req, res) => {
    const { email, password } = req.body;

    try {
      // Ask Supabase to verify the email and password
      const { data, error } = await supabase.auth.signInWithPassword({
        email: email,
        password: password
      });

      if (error) throw error;

      // If successful, hand the secure session Token (JWT) back to Unity
      return res.status(200).json({
        message: 'Login successful!',
        token: data.session.access_token, // The digital key Unity must save
        userId: data.user.id              // The player's unique ID
      });

    } catch (error) {
      // If password or email is incorrect, return a clean error message
      return res.status(400).json({ error: error.message });
    }
  });

  return router;
}