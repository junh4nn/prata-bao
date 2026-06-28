export default function createAuthMiddleware(supabaseAuth) {
  return async function requireAuth(req, res, next) {
    const header = req.headers.authorization || '';
    const token = header.startsWith('Bearer ') ? header.slice(7) : null;
    if (!token) return res.status(401).json({ error: 'Missing or invalid Authorization header' });

    const { data, error } = await supabaseAuth.auth.getUser(token);
    if (error || !data?.user) return res.status(401).json({ error: 'Invalid or expired session' });

    req.userId = data.user.id;
    next();
  };
}
