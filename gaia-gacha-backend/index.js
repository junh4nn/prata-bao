// index.js: the entry point. Loads environment variables, then imports the
// configured app from app.js (its closest collaborator) and starts listening.

import 'dotenv/config';
import app from './app.js';

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));