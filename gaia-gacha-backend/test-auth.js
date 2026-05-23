import fetch from 'node-fetch';

const BASE_URL = 'http://localhost:3000/api/auth';

// Generate a random email so we don't get "Email already exists" errors when testing repeatedly
const testEmail = `player_${Math.floor(Math.random() * 10000)}@test.com`;
const testPassword = 'SecurePassword123!';

async function runAuthTest() {
  console.log('--- STARTING BACKEND AUTH FLOW TEST ---');
  console.log(`Testing with Target Email: ${testEmail}\n`);

  // =====================================
  // TEST 1: REGISTER ACCOUNT
  // =====================================
  try {
    console.log('[TEST 1] Sending registration request...');
    const registerResponse = await fetch(`${BASE_URL}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: testEmail, password: testPassword })
    });

    const registerData = await registerResponse.json();
    console.log(`Status: ${registerResponse.status}`);
    console.log('Response:', registerData, '\n');

    if (registerResponse.status !== 201) {
      console.error('❌ Registration test failed. Aborting login test.');
      return;
    }
  } catch (err) {
    console.error('❌ Network Connection Error:', err.message);
    return;
  }

  // =====================================
  // TEST 2: LOGIN ACCOUNT
  // =====================================
  try {
    console.log('[TEST 2] Sending login request...');
    const loginResponse = await fetch(`${BASE_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: testEmail, password: testPassword })
    });

    const loginData = await loginResponse.json();
    console.log(`Status: ${loginResponse.status}`);
    console.log('Response:', loginData);

    if (loginResponse.status === 200 && loginData.token) {
      console.log('\n✅ SUCCESS: Secure login token returned safely from Supabase!');
    } else {
      console.error('\n❌ Login test failed.');
    }
  } catch (err) {
    console.error('❌ Network Connection Error during login:', err.message);
  }
}

runAuthTest();