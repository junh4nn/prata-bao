# Seed Data Regeneration

How to regenerate `supabase/seed.sql`, which holds only static reference data (`items`, `quiz_questions`). Live player data (`profiles`, `inventory`) and all `auth.*`/`storage.*` schemas must never appear in this file.

## Regenerating

Requires the Supabase CLI to be linked to the source project (usually prod):

    supabase db dump --data-only -s public -x public.profiles -x public.inventory --file supabase/seed.sql

- `-s public` scopes the dump to the `public` schema only, so `auth.*` data (user accounts, sessions, tokens) is never captured.
- `-x public.profiles -x public.inventory` excludes live player data, which changes constantly and should not be checked into version control.

Adding a new **static** table (like `items`)? No changes needed here, it's included automatically as long as it's in the `public` schema and not excluded above.

Adding a new **live/dynamic** table instead? Add `-x public.<table>` to the command above and regenerate.

## After regenerating: convert to upserts

`db dump` only ever produces plain `INSERT INTO ... VALUES (...)` statements. These must be manually converted to `ON CONFLICT (id) DO UPDATE` upserts, so `supabase db push --include-seed` can be re-run safely against a database that already has rows (a plain `INSERT` would otherwise fail with duplicate-key errors).

**`items`**
```sql
ON CONFLICT (id) DO UPDATE SET
	name = EXCLUDED.name,
	rarity = EXCLUDED.rarity,
	weight = EXCLUDED.weight,
	type = EXCLUDED.type;
```

**`quiz_questions`**
```sql
ON CONFLICT (id) DO UPDATE SET
	question_text = EXCLUDED.question_text,
	option_a = EXCLUDED.option_a,
	option_b = EXCLUDED.option_b,
	option_c = EXCLUDED.option_c,
	correct_index = EXCLUDED.correct_index,
	explanation = EXCLUDED.explanation,
	reward_coins = EXCLUDED.reward_coins,
	is_active = EXCLUDED.is_active;
```

If either table's columns change, update the corresponding `ON CONFLICT` clause above to match.
