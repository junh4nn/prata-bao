// quiz.test.js: integration tests for quiz.js's GET /questions and POST /answer
// routes, against a real local Supabase instance. Most cases use a stub auth
// middleware; one uses the real app.js wiring with a real token.

import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import request from 'supertest';
import express from 'express';

import quizRouter from '../../routes/quiz.js';
import realApp from '../../app.js';
import { supabase } from './helpers/supabaseClients.js';
import { createStubAuth } from './helpers/stubAuth.js';
import { getOrCreateTestUser } from './helpers/testUser.js';

const STARTING_COINS = 250;
const CORRECT_QUESTION_ID = 1;
const CORRECT_INDEX = 2;
const REWARD_COINS = 10;
const NONEXISTENT_QUESTION_ID = 999999;

let testUserId;
let testUserToken;
let testApp;

beforeAll(async () => {
  const testUser = await getOrCreateTestUser();
  testUserId = testUser.userId;
  testUserToken = testUser.token;

  testApp = express();
  testApp.use(express.json());
  testApp.use('/api/quiz', quizRouter(supabase, createStubAuth(testUserId)));
});

describe('GET /api/quiz/questions', () => {
  it('returns exactly 3 questions, each with 3 shuffled options', async () => {
    const res = await request(testApp).get('/api/quiz/questions');

    expect(res.status).toBe(200);
    expect(res.body.questions).toHaveLength(3);
    for (const question of res.body.questions) {
      expect(question).toHaveProperty('id');
      expect(question).toHaveProperty('questionText');
      expect(question.options).toHaveLength(3);
    }
  });
});

describe('POST /api/quiz/answer', () => {
  afterEach(async () => {
    await supabase.from('profiles').update({ coins: STARTING_COINS }).eq('id', testUserId);
  });

  it('awards coins on a correct answer', async () => {
    const res = await request(testApp)
      .post('/api/quiz/answer')
      .send({ questionId: CORRECT_QUESTION_ID, selectedIndex: CORRECT_INDEX });

    expect(res.status).toBe(200);
    expect(res.body.isCorrect).toBe(true);
    expect(res.body.coinsEarned).toBe(REWARD_COINS);
    expect(res.body.newBalance).toBe(STARTING_COINS + REWARD_COINS);

    const { data: profile } = await supabase
      .from('profiles')
      .select('coins')
      .eq('id', testUserId)
      .single();
    expect(profile.coins).toBe(STARTING_COINS + REWARD_COINS);
  });

  it('does not award coins on an incorrect answer', async () => {
    const wrongIndex = (CORRECT_INDEX + 1) % 3;
    const res = await request(testApp)
      .post('/api/quiz/answer')
      .send({ questionId: CORRECT_QUESTION_ID, selectedIndex: wrongIndex });

    expect(res.status).toBe(200);
    expect(res.body.isCorrect).toBe(false);
    expect(res.body.coinsEarned).toBe(0);
    expect(res.body.newBalance).toBe(STARTING_COINS);

    const { data: profile } = await supabase
      .from('profiles')
      .select('coins')
      .eq('id', testUserId)
      .single();
    expect(profile.coins).toBe(STARTING_COINS);
  });

  it('returns 404 for a bad questionId', async () => {
    const res = await request(testApp)
      .post('/api/quiz/answer')
      .send({ questionId: NONEXISTENT_QUESTION_ID, selectedIndex: 0 });

    expect(res.status).toBe(404);
  });

  it('works through the real app.js wiring with a real token', async () => {
    const res = await request(realApp)
      .post('/api/quiz/answer')
      .set('Authorization', `Bearer ${testUserToken}`)
      .send({ questionId: CORRECT_QUESTION_ID, selectedIndex: CORRECT_INDEX });

    expect(res.status).toBe(200);
    expect(res.body.isCorrect).toBe(true);
    expect(res.body.newBalance).toBe(STARTING_COINS + REWARD_COINS);
  });
});
