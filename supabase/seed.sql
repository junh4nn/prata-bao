SET session_replication_role = replica;

--
-- PostgreSQL database dump
--

-- \restrict ac0jIPuXrFcU4CcFuiniX2ara2gqcTzkVAxinGbTDnTdUmenswzLihxmAUaKqKq

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
-- Data for Name: audit_log_entries; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: custom_oauth_providers; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: flow_state; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: users; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--

INSERT INTO "auth"."users" ("instance_id", "id", "aud", "role", "email", "encrypted_password", "email_confirmed_at", "invited_at", "confirmation_token", "confirmation_sent_at", "recovery_token", "recovery_sent_at", "email_change_token_new", "email_change", "email_change_sent_at", "last_sign_in_at", "raw_app_meta_data", "raw_user_meta_data", "is_super_admin", "created_at", "updated_at", "phone", "phone_confirmed_at", "phone_change", "phone_change_token", "phone_change_sent_at", "email_change_token_current", "email_change_confirm_status", "banned_until", "reauthentication_token", "reauthentication_sent_at", "is_sso_user", "deleted_at", "is_anonymous") VALUES
	('00000000-0000-0000-0000-000000000000', '4e0a6f18-7132-473a-942e-df705f4cd4bf', 'authenticated', 'authenticated', '123@gmail.com', '$2a$10$kvB6DVSy3jUIvmn.9P7UvuiDs8XUON8QdTCbJbfhutrGWUNdBxudG', '2026-07-01 07:19:43.213533+00', NULL, '', NULL, '', NULL, '', '', NULL, '2026-07-01 07:23:56.968925+00', '{"provider": "email", "providers": ["email"]}', '{"email_verified": true}', NULL, '2026-07-01 07:19:43.144103+00', '2026-07-01 07:23:56.986043+00', NULL, NULL, '', '', NULL, '', 0, NULL, '', NULL, false, NULL, false),
	('00000000-0000-0000-0000-000000000000', '46480c9f-9073-45ec-b21e-3cbde9b1f6d9', 'authenticated', 'authenticated', 'test2@shise.com', '$2a$10$D9Xu5O0yK//hRvEqZsaFoOGaOazth2SyvPRjb7XtLy1B4jgq.wDWW', '2026-06-29 01:18:59.722297+00', NULL, '', NULL, '', NULL, '', '', NULL, '2026-06-29 01:19:01.271799+00', '{"provider": "email", "providers": ["email"]}', '{"email_verified": true}', NULL, '2026-06-29 01:18:59.698955+00', '2026-06-29 01:19:01.278299+00', NULL, NULL, '', '', NULL, '', 0, NULL, '', NULL, false, NULL, false),
	('00000000-0000-0000-0000-000000000000', '945051fb-7cdb-40ff-937b-cac7c63c7a34', 'authenticated', 'authenticated', 'test@mail.com', '$2a$10$fZ5FQJcOdnFf4bz27UbpzO9FnETYywDrlulthcc6Vb1nqvN9/zk1u', '2026-07-05 16:20:02.937161+00', NULL, '', NULL, '', NULL, '', '', NULL, '2026-07-05 16:20:04.703399+00', '{"provider": "email", "providers": ["email"]}', '{"email_verified": true}', NULL, '2026-07-05 16:20:02.878271+00', '2026-07-06 08:50:15.989358+00', NULL, NULL, '', '', NULL, '', 0, NULL, '', NULL, false, NULL, false),
	('00000000-0000-0000-0000-000000000000', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', 'authenticated', 'authenticated', 'test1@shise.com', '$2a$10$TKIVi2S/MYPSv8pKjack8uWjsWkeR8MmHEN4EoB0epFrKEmg.ciqm', '2026-06-30 05:34:07.240774+00', NULL, '', NULL, '', NULL, '', '', NULL, '2026-07-06 14:07:27.46649+00', '{"provider": "email", "providers": ["email"]}', '{"email_verified": true}', NULL, '2026-06-30 05:34:07.211473+00', '2026-07-06 14:07:27.500963+00', NULL, NULL, '', '', NULL, '', 0, NULL, '', NULL, false, NULL, false),
	('00000000-0000-0000-0000-000000000000', '559b32dd-9661-4818-b3f8-fed43446c197', 'authenticated', 'authenticated', 'abc@def.com', '$2a$10$Lq47xYtydp0GZec4tTQv0O5L4v0jr/nj4B8rGFX.SYQ6Da/W0Sszy', '2026-06-30 07:48:57.30823+00', NULL, '', NULL, '', NULL, '', '', NULL, '2026-06-30 08:35:39.652019+00', '{"provider": "email", "providers": ["email"]}', '{"email_verified": true}', NULL, '2026-06-30 07:48:57.269875+00', '2026-06-30 08:35:39.660605+00', NULL, NULL, '', '', NULL, '', 0, NULL, '', NULL, false, NULL, false);


--
-- Data for Name: identities; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--

INSERT INTO "auth"."identities" ("provider_id", "user_id", "identity_data", "provider", "last_sign_in_at", "created_at", "updated_at", "id") VALUES
	('46480c9f-9073-45ec-b21e-3cbde9b1f6d9', '46480c9f-9073-45ec-b21e-3cbde9b1f6d9', '{"sub": "46480c9f-9073-45ec-b21e-3cbde9b1f6d9", "email": "test2@shise.com", "email_verified": false, "phone_verified": false}', 'email', '2026-06-29 01:18:59.720488+00', '2026-06-29 01:18:59.720556+00', '2026-06-29 01:18:59.720556+00', '4bc58b10-cbce-432c-8c1f-b12c1dd40047'),
	('4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', '{"sub": "4ec4c118-57a7-4ecd-bfd2-682da4b4dde6", "email": "test1@shise.com", "email_verified": false, "phone_verified": false}', 'email', '2026-06-30 05:34:07.234382+00', '2026-06-30 05:34:07.234441+00', '2026-06-30 05:34:07.234441+00', '7d299653-ce88-4190-b36d-b20d604db7df'),
	('559b32dd-9661-4818-b3f8-fed43446c197', '559b32dd-9661-4818-b3f8-fed43446c197', '{"sub": "559b32dd-9661-4818-b3f8-fed43446c197", "email": "abc@def.com", "email_verified": false, "phone_verified": false}', 'email', '2026-06-30 07:48:57.301717+00', '2026-06-30 07:48:57.301774+00', '2026-06-30 07:48:57.301774+00', '4f171140-8776-45fb-a96b-8533e15eb58d'),
	('4e0a6f18-7132-473a-942e-df705f4cd4bf', '4e0a6f18-7132-473a-942e-df705f4cd4bf', '{"sub": "4e0a6f18-7132-473a-942e-df705f4cd4bf", "email": "123@gmail.com", "email_verified": false, "phone_verified": false}', 'email', '2026-07-01 07:19:43.201718+00', '2026-07-01 07:19:43.201771+00', '2026-07-01 07:19:43.201771+00', 'e4eb6f56-cfa7-42c8-85a2-9f3336ade75a'),
	('945051fb-7cdb-40ff-937b-cac7c63c7a34', '945051fb-7cdb-40ff-937b-cac7c63c7a34', '{"sub": "945051fb-7cdb-40ff-937b-cac7c63c7a34", "email": "test@mail.com", "email_verified": false, "phone_verified": false}', 'email', '2026-07-05 16:20:02.922444+00', '2026-07-05 16:20:02.925705+00', '2026-07-05 16:20:02.925705+00', 'a1ad956d-08a2-40b6-88e5-49fd07904acd');


--
-- Data for Name: instances; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: oauth_clients; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: sessions; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--

INSERT INTO "auth"."sessions" ("id", "user_id", "created_at", "updated_at", "factor_id", "aal", "not_after", "refreshed_at", "user_agent", "ip", "tag", "oauth_client_id", "refresh_token_hmac_key", "refresh_token_counter", "scopes") VALUES
	('89d2ed9b-26ca-4e00-bff4-85de35002121', '46480c9f-9073-45ec-b21e-3cbde9b1f6d9', '2026-06-29 01:19:01.273054+00', '2026-06-29 01:19:01.273054+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('e5513de2-039f-45f2-85a7-f1e04881ee9f', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', '2026-06-30 05:34:09.699081+00', '2026-06-30 05:34:09.699081+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('24572dfb-0874-4ec0-b7de-f69e9332b6d6', '559b32dd-9661-4818-b3f8-fed43446c197', '2026-06-30 07:49:00.789994+00', '2026-06-30 07:49:00.789994+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('68b0b665-b7b5-4fc4-8bc3-dfee8fd90be8', '559b32dd-9661-4818-b3f8-fed43446c197', '2026-06-30 08:29:41.482287+00', '2026-06-30 08:29:41.482287+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('e1bbb66e-5d28-40ab-8445-3204333cbe17', '559b32dd-9661-4818-b3f8-fed43446c197', '2026-06-30 08:29:53.328473+00', '2026-06-30 08:29:53.328473+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('8979bf6e-5db7-46b1-8b84-5598980d5391', '559b32dd-9661-4818-b3f8-fed43446c197', '2026-06-30 08:35:39.652178+00', '2026-06-30 08:35:39.652178+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('5a78d04f-94d8-4a43-bb35-34e51337dbe2', '4e0a6f18-7132-473a-942e-df705f4cd4bf', '2026-07-01 07:19:48.130182+00', '2026-07-01 07:19:48.130182+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('cfb6d502-b739-49d7-88a8-98ec2096de22', '4e0a6f18-7132-473a-942e-df705f4cd4bf', '2026-07-01 07:23:56.969022+00', '2026-07-01 07:23:56.969022+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL),
	('17be0319-8ba4-4427-8960-7bbe2fd9251c', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', '2026-07-04 17:33:54.914746+00', '2026-07-04 17:33:54.914746+00', NULL, 'aal1', NULL, NULL, 'node', '121.7.202.185', NULL, NULL, NULL, NULL, NULL),
	('bf82e049-7b7f-4b16-b5ec-3235681e03d2', '945051fb-7cdb-40ff-937b-cac7c63c7a34', '2026-07-05 16:20:04.705042+00', '2026-07-06 08:50:15.999527+00', NULL, 'aal1', NULL, '2026-07-06 08:50:15.999412', 'node', '14.100.31.211', NULL, NULL, NULL, NULL, NULL),
	('5804068b-97c8-4fc0-b24b-7c149bc85fe0', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', '2026-07-06 14:07:27.466602+00', '2026-07-06 14:07:27.466602+00', NULL, 'aal1', NULL, NULL, 'node', '167.88.158.108', NULL, NULL, NULL, NULL, NULL);


--
-- Data for Name: mfa_amr_claims; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--

INSERT INTO "auth"."mfa_amr_claims" ("session_id", "created_at", "updated_at", "authentication_method", "id") VALUES
	('89d2ed9b-26ca-4e00-bff4-85de35002121', '2026-06-29 01:19:01.278853+00', '2026-06-29 01:19:01.278853+00', 'password', '9825d315-d98f-4f63-b20f-97a34ebf2704'),
	('e5513de2-039f-45f2-85a7-f1e04881ee9f', '2026-06-30 05:34:09.724899+00', '2026-06-30 05:34:09.724899+00', 'password', 'f880088a-8f92-499b-8f55-7afa38d3f98f'),
	('24572dfb-0874-4ec0-b7de-f69e9332b6d6', '2026-06-30 07:49:00.81842+00', '2026-06-30 07:49:00.81842+00', 'password', '758da15b-0210-4538-898b-a60f4c5c1a89'),
	('68b0b665-b7b5-4fc4-8bc3-dfee8fd90be8', '2026-06-30 08:29:41.509289+00', '2026-06-30 08:29:41.509289+00', 'password', '43dc95d3-4f4d-4884-9087-1cb25500d110'),
	('e1bbb66e-5d28-40ab-8445-3204333cbe17', '2026-06-30 08:29:53.333307+00', '2026-06-30 08:29:53.333307+00', 'password', '972d7faf-0e7d-44e9-b71e-e560862552e4'),
	('8979bf6e-5db7-46b1-8b84-5598980d5391', '2026-06-30 08:35:39.662799+00', '2026-06-30 08:35:39.662799+00', 'password', 'a6207400-38f6-4aa0-b735-bcc716655d98'),
	('5a78d04f-94d8-4a43-bb35-34e51337dbe2', '2026-07-01 07:19:48.168053+00', '2026-07-01 07:19:48.168053+00', 'password', '7f424fe0-b08b-427b-a58d-52929f671b8d'),
	('cfb6d502-b739-49d7-88a8-98ec2096de22', '2026-07-01 07:23:56.989222+00', '2026-07-01 07:23:56.989222+00', 'password', 'f86531ff-daf7-4400-aa0d-41c77cc8bfcc'),
	('17be0319-8ba4-4427-8960-7bbe2fd9251c', '2026-07-04 17:33:54.947311+00', '2026-07-04 17:33:54.947311+00', 'password', 'a380a541-0b16-42f3-9c0d-e5f6a339282f'),
	('bf82e049-7b7f-4b16-b5ec-3235681e03d2', '2026-07-05 16:20:04.737617+00', '2026-07-05 16:20:04.737617+00', 'password', '51a3184a-eff4-4165-b612-2f5a1731f2f0'),
	('5804068b-97c8-4fc0-b24b-7c149bc85fe0', '2026-07-06 14:07:27.507912+00', '2026-07-06 14:07:27.507912+00', 'password', '9d3d0902-2122-46b4-9349-933f0cda2235');


--
-- Data for Name: mfa_factors; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: mfa_challenges; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: oauth_authorizations; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: oauth_client_states; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: oauth_consents; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: one_time_tokens; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: refresh_tokens; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--

INSERT INTO "auth"."refresh_tokens" ("instance_id", "id", "token", "user_id", "revoked", "created_at", "updated_at", "parent", "session_id") VALUES
	('00000000-0000-0000-0000-000000000000', 504, 'bqiemiremosn', '46480c9f-9073-45ec-b21e-3cbde9b1f6d9', false, '2026-06-29 01:19:01.276998+00', '2026-06-29 01:19:01.276998+00', NULL, '89d2ed9b-26ca-4e00-bff4-85de35002121'),
	('00000000-0000-0000-0000-000000000000', 505, 'gy25b53aum2l', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', false, '2026-06-30 05:34:09.71163+00', '2026-06-30 05:34:09.71163+00', NULL, 'e5513de2-039f-45f2-85a7-f1e04881ee9f'),
	('00000000-0000-0000-0000-000000000000', 506, 'rki3pn5btfga', '559b32dd-9661-4818-b3f8-fed43446c197', false, '2026-06-30 07:49:00.806167+00', '2026-06-30 07:49:00.806167+00', NULL, '24572dfb-0874-4ec0-b7de-f69e9332b6d6'),
	('00000000-0000-0000-0000-000000000000', 507, 'tb7i5pf373m4', '559b32dd-9661-4818-b3f8-fed43446c197', false, '2026-06-30 08:29:41.498018+00', '2026-06-30 08:29:41.498018+00', NULL, '68b0b665-b7b5-4fc4-8bc3-dfee8fd90be8'),
	('00000000-0000-0000-0000-000000000000', 508, '5rsurl6jhrrk', '559b32dd-9661-4818-b3f8-fed43446c197', false, '2026-06-30 08:29:53.329573+00', '2026-06-30 08:29:53.329573+00', NULL, 'e1bbb66e-5d28-40ab-8445-3204333cbe17'),
	('00000000-0000-0000-0000-000000000000', 509, 'zl52ygm6gz2t', '559b32dd-9661-4818-b3f8-fed43446c197', false, '2026-06-30 08:35:39.658222+00', '2026-06-30 08:35:39.658222+00', NULL, '8979bf6e-5db7-46b1-8b84-5598980d5391'),
	('00000000-0000-0000-0000-000000000000', 510, 'hq7fuqwpxerl', '4e0a6f18-7132-473a-942e-df705f4cd4bf', false, '2026-07-01 07:19:48.14825+00', '2026-07-01 07:19:48.14825+00', NULL, '5a78d04f-94d8-4a43-bb35-34e51337dbe2'),
	('00000000-0000-0000-0000-000000000000', 511, 'tcblbycai3h2', '4e0a6f18-7132-473a-942e-df705f4cd4bf', false, '2026-07-01 07:23:56.983671+00', '2026-07-01 07:23:56.983671+00', NULL, 'cfb6d502-b739-49d7-88a8-98ec2096de22'),
	('00000000-0000-0000-0000-000000000000', 512, '7wvgwqifb2i4', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', false, '2026-07-04 17:33:54.92737+00', '2026-07-04 17:33:54.92737+00', NULL, '17be0319-8ba4-4427-8960-7bbe2fd9251c'),
	('00000000-0000-0000-0000-000000000000', 513, 'ayrt5xi335iy', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 16:20:04.722641+00', '2026-07-05 17:18:21.499396+00', NULL, 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 514, 'tgzjpzhocgbp', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 17:18:21.510132+00', '2026-07-05 18:16:51.735218+00', 'ayrt5xi335iy', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 515, 'z25zophpmbip', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 18:16:51.743859+00', '2026-07-05 19:15:21.58655+00', 'tgzjpzhocgbp', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 516, 'py7r2qiovk3h', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 19:15:21.592276+00', '2026-07-05 20:13:21.827579+00', 'z25zophpmbip', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 517, 'i6a3pd5aeawu', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 20:13:21.832732+00', '2026-07-05 21:11:21.963901+00', 'py7r2qiovk3h', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 518, 'k3ryp7dbbcdr', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 21:11:21.971473+00', '2026-07-05 22:09:22.147067+00', 'i6a3pd5aeawu', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 519, 'wef3hpgydixx', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 22:09:22.151147+00', '2026-07-05 23:07:52.336378+00', 'k3ryp7dbbcdr', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 520, 'q7qtih7hdh2k', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-05 23:07:52.341583+00', '2026-07-06 00:06:22.549406+00', 'wef3hpgydixx', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 521, '2hd3xst5nxku', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 00:06:22.557654+00', '2026-07-06 01:04:23.796429+00', 'q7qtih7hdh2k', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 522, '3eazenmy252f', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 01:04:23.803333+00', '2026-07-06 02:02:52.948396+00', '2hd3xst5nxku', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 523, 'tujvklurszjx', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 02:02:52.956469+00', '2026-07-06 03:00:53.120437+00', '3eazenmy252f', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 524, 'i7545bpig64j', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 03:00:53.12464+00', '2026-07-06 03:59:23.30412+00', 'tujvklurszjx', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 525, 'l4znlh5fu5tk', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 03:59:23.311462+00', '2026-07-06 04:57:53.557941+00', 'i7545bpig64j', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 526, 'woflz547minp', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 04:57:53.563004+00', '2026-07-06 05:55:53.912569+00', 'l4znlh5fu5tk', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 527, 'noa4pg5bhhnl', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 05:55:53.919264+00', '2026-07-06 06:53:53.930957+00', 'woflz547minp', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 528, 'gttkfxxu46i7', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 06:53:53.937182+00', '2026-07-06 07:51:54.13919+00', 'noa4pg5bhhnl', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 529, 'ii4mgyn2uyum', '945051fb-7cdb-40ff-937b-cac7c63c7a34', true, '2026-07-06 07:51:54.147403+00', '2026-07-06 08:50:15.975021+00', 'gttkfxxu46i7', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 530, 'edkkjhnwbo2u', '945051fb-7cdb-40ff-937b-cac7c63c7a34', false, '2026-07-06 08:50:15.984724+00', '2026-07-06 08:50:15.984724+00', 'ii4mgyn2uyum', 'bf82e049-7b7f-4b16-b5ec-3235681e03d2'),
	('00000000-0000-0000-0000-000000000000', 531, 'thjdwu35fnvt', '4ec4c118-57a7-4ecd-bfd2-682da4b4dde6', false, '2026-07-06 14:07:27.486967+00', '2026-07-06 14:07:27.486967+00', NULL, '5804068b-97c8-4fc0-b24b-7c149bc85fe0');


--
-- Data for Name: sso_providers; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: saml_providers; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: saml_relay_states; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: sso_domains; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: webauthn_challenges; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: webauthn_credentials; Type: TABLE DATA; Schema: auth; Owner: supabase_auth_admin
--



--
-- Data for Name: items; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO "public"."items" ("id", "name", "rarity", "weight", "type") OVERRIDING SYSTEM VALUE VALUES
	(1, 'Mangrove Seed', 'Common', 70, 'Flora'),
	(2, 'Coral Fragment', 'Rare', 25, 'Fauna'),
	(3, 'Giant Sea Turtle Shell', 'Legendary', 5, 'Fauna');


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
	(10, 'Which of these human activities contributes most to biodiversity loss globally?', 'Pollution', 'Overexploitation of species', 'Habitat destruction', 2, 'Habitat destruction, including clearing land for agriculture, logging, and urban development, is the single largest driver of species extinction worldwide.', 10, true);


--
-- Data for Name: buckets; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Data for Name: buckets_analytics; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Data for Name: buckets_vectors; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Data for Name: objects; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Data for Name: s3_multipart_uploads; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Data for Name: s3_multipart_uploads_parts; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Data for Name: vector_indexes; Type: TABLE DATA; Schema: storage; Owner: supabase_storage_admin
--



--
-- Name: refresh_tokens_id_seq; Type: SEQUENCE SET; Schema: auth; Owner: supabase_auth_admin
--

SELECT pg_catalog.setval('"auth"."refresh_tokens_id_seq"', 531, true);


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

-- \unrestrict ac0jIPuXrFcU4CcFuiniX2ara2gqcTzkVAxinGbTDnTdUmenswzLihxmAUaKqKq

RESET ALL;
