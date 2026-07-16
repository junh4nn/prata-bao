SET session_replication_role = replica;

--
-- PostgreSQL database dump
--

-- \restrict jx5Jo7sqsDBUrJwv4fBTYHVtyMRbG0hyo2hQDW0TmChif4tubCvgQvXAl4xGxLe

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: items; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "public"."items" ("id", "name", "rarity", "weight", "type") OVERRIDING SYSTEM VALUE VALUES
	(1, 'Mangrove Seed', 'Common', 70, 'Flora'),
	(2, 'Coral Fragment', 'Rare', 25, 'Fauna'),
	(3, 'Giant Sea Turtle Shell', 'Legendary', 5, 'Fauna')
ON CONFLICT (id) DO UPDATE SET
	name = EXCLUDED.name,
	rarity = EXCLUDED.rarity,
	weight = EXCLUDED.weight,
	type = EXCLUDED.type;	

--
-- Data for Name: quiz_questions; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "public"."quiz_questions" ("id", "question_text", "option_a", "option_b", "option_c", "correct_index", "explanation", "reward_coins", "is_active") VALUES
	(1, 'Which gas do plants absorb during photosynthesis?', 'Oxygen', 'Nitrogen', 'Carbon Dioxide', 2, 'Plants absorb CO₂ and use sunlight to convert it into glucose, releasing oxygen as a byproduct.', 10, true),
	(2, 'What term describes an ecosystem with extremely low rainfall, sparse vegetation, and extreme temperatures?', 'Tundra', 'Savanna', 'Desert', 2, 'Deserts receive less than 250mm of rain per year and are defined by aridity rather than heat, so even Antarctica qualifies.', 10, true),
	(3, 'Which ocean zone receives no sunlight at all?', 'Mesopelagic zone', 'Bathypelagic zone', 'Hadal zone', 2, 'The hadal zone (6,000 to 11,000m deep, found in ocean trenches) is completely lightless and one of Earth''s most extreme environments.', 10, true),
	(4, 'What is the term for a species that has a disproportionately large effect on its ecosystem relative to its abundance?', 'Invasive species', 'Apex predator', 'Keystone species', 2, 'Keystone species like sea otters or wolves hold ecosystems together, and their removal causes dramatic cascading changes.', 10, true),
	(5, 'Bees are critical pollinators. Approximately what fraction of the world''s food supply depends on animal pollination?', 'Two thirds', 'One tenth', 'One third', 2, 'Around one third of global food production relies on pollinators, including fruits, vegetables, nuts, and seeds.', 10, true),
	(6, 'What natural phenomenon is caused by the buildup of greenhouse gases trapping heat in Earth''s atmosphere?', 'The ozone effect', 'Acid rain', 'The greenhouse effect', 2, 'Greenhouse gases like CO₂ and methane absorb outgoing infrared radiation, warming the planet in a process intensified by human activity.', 10, true),
	(7, 'Which biome stores the most carbon per unit area on land?', 'Tropical rainforest', 'Temperate forest', 'Peatland', 2, 'Despite covering only 3% of land, peatlands store twice as much carbon as all forests combined, making their preservation critical.', 10, true),
	(8, 'What is biomimicry?', 'Cloning endangered animals to restore populations', 'Using drones to monitor wildlife habitats', 'Designing solutions inspired by patterns found in nature', 2, 'Biomimicry draws from nature''s 3.8 billion years of evolution. Velcro was inspired by burr hooks, and bullet train noses by kingfisher beaks.', 10, true),
	(9, 'The Great Barrier Reef is experiencing mass bleaching events. What triggers coral bleaching?', 'Ocean acidification dissolving coral calcium', 'Overfishing removing herbivores that clean coral', 'Warmer water temperatures causing corals to expel their algae', 2, 'When water gets too warm, corals expel the symbiotic zooxanthellae algae that give them colour and nutrients, leaving them white and vulnerable.', 10, true),
	(10, 'Which of these human activities contributes most to biodiversity loss globally?', 'Pollution', 'Overexploitation of species', 'Habitat destruction', 2, 'Habitat destruction, including clearing land for agriculture, logging, and urban development, is the single largest driver of species extinction worldwide.', 10, true)
ON CONFLICT (id) DO UPDATE SET
	question_text = EXCLUDED.question_text,
	option_a = EXCLUDED.option_a,
	option_b = EXCLUDED.option_b,
	option_c = EXCLUDED.option_c,
	correct_index = EXCLUDED.correct_index,
	explanation = EXCLUDED.explanation,
	reward_coins = EXCLUDED.reward_coins,
	is_active = EXCLUDED.is_active;

--
-- Name: items_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"public"."items_id_seq"', 3, true);


--
-- Name: quiz_questions_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('"public"."quiz_questions_id_seq"', 10, true);


--
-- PostgreSQL database dump complete
--

-- \unrestrict jx5Jo7sqsDBUrJwv4fBTYHVtyMRbG0hyo2hQDW0TmChif4tubCvgQvXAl4xGxLe

RESET ALL;
