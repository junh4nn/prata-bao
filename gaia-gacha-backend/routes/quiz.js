import express from 'express';

export default function (supabase) {
  const router = express.Router();

  function shuffle(array) {
    const result = [...array];
    for (let i = result.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [result[i], result[j]] = [result[j], result[i]];
    }
    return result;
  }

  // --- FETCH A RANDOM BATCH OF QUESTIONS ---
  router.get('/questions', async (req, res) => {
    const { data: questions, error } = await supabase
      .from('quiz_questions')
      .select('id, question_text, option_a, option_b, option_c')
      .eq('is_active', true);

    if (error || !questions || questions.length === 0) {
      console.error("Fetch Quiz Questions Error:", error?.message);
      return res.status(500).json({ error: "Failed to load quiz questions" });
    }

    const selected = shuffle(questions).slice(0, 3);

    const result = selected.map(q => ({
      id: q.id,
      questionText: q.question_text,
      options: shuffle([
        { key: 0, text: q.option_a },
        { key: 1, text: q.option_b },
        { key: 2, text: q.option_c }
      ])
    }));

    res.json({ questions: result });
  });

  // --- GRADE AN ANSWER AND AWARD COINS ---
  router.post('/answer', async (req, res) => {
    const { userId, questionId, selectedIndex } = req.body;

    try {
      // 1. Fetch Question Data
      const { data: question, error: questionError } = await supabase
        .from('quiz_questions')
        .select('correct_index, explanation, reward_coins')
        .eq('id', questionId)
        .single();

      if (questionError || !question) {
        console.error("Fetch Quiz Question Error:", questionError?.message);
        return res.status(404).json({ error: "Question not found" });
      }

      // 2. Fetch Player Data
      const { data: player, error: playerError } = await supabase
        .from('profiles')
        .select('coins')
        .eq('id', userId)
        .single();

      if (playerError || !player) {
        console.error("Fetch Player Error:", playerError?.message);
        return res.status(404).json({ error: "Player not found" });
      }

      // 3. Grade the Answer
      const isCorrect = selectedIndex === question.correct_index;
      const coinsEarned = isCorrect ? question.reward_coins : 0;
      const newBalance = player.coins + coinsEarned;

      // 4. Award Coins (only on a correct answer)
      if (isCorrect) {
        const { error: updateError } = await supabase
          .from('profiles')
          .update({ coins: newBalance })
          .eq('id', userId);

        if (updateError) {
          console.error("Award Coins Database Error:", updateError.message);
          return res.status(500).json({ error: "Failed to award coins" });
        }
      }

      // 5. Send Result back to Unity
      return res.json({
        isCorrect,
        correctIndex: question.correct_index,
        explanation: question.explanation,
        coinsEarned,
        newBalance
      });

    } catch (err) {
      console.error("System Route Exception Caught:", err);
      return res.status(500).json({ error: "Server Error" });
    }
  });

  return router;
}
