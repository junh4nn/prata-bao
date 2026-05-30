# Milestone I Submission

## Team Name:
Prata Bao

## Proposed Level of Achievement:
Artemis

## Motivation

Gacha games are enormously popular, yet most players engage with them without understanding the mechanics underneath — the weighted randomness, the secure coin economy, the real-time sync between client and server. As fans of these games ourselves, we wanted to build one from the ground up to understand exactly how they work.

At the same time, most gacha games offer no real-world value beyond entertainment. We believe that **learning and fun are not mutually exclusive** — and that the engagement loop of a gacha game is a powerful vehicle for education. We wanted to combine the addictive collection mechanic with genuine ecology knowledge: every item a player pulls, every quiz question they answer, is an opportunity to discover something real about the natural world. Our goal is for players to walk away knowing what a mangrove is, why coral reefs matter, or why sea turtles are endangered — not because they studied, but because they were playing.

The technical challenge is also compelling: building a gacha game requires solving a full-stack problem — a Unity game client, a secure Node.js backend that protects sensitive operations like coin deductions, and a live Supabase database — all working in real time. This is a combination rarely tackled in student projects.

## Vision

Gaia Gacha will be a PC/mobile-ready ecology-themed gacha game built in Unity, backed by a Node.js REST API and Supabase.

Players register an account, log in, and spend **Eco-Coins** on gacha pulls to discover ecology specimens of varying rarity. The rarity system is designed to mirror real-world conservation scarcity — common species appear frequently, while endangered or rare specimens are harder to find. A quiz system lets players earn Eco-Coins by correctly answering ecology questions, creating a loop where **learning is directly rewarded**.

Specific features are elaborated in the following sections.

Ultimately, we envision Gaia Gacha as a game that proves learning can be genuinely fun — where players build their ecology knowledge naturally, through play, without it ever feeling like studying. The gacha loop, the quiz rewards, and the ecology-themed collection all serve this single intention: make the natural world exciting and worth knowing about.

## User Profiling

Our primary target audience is **casual gamers aged 15–30** who enjoy collection and gacha games (e.g. Genshin Impact, Pokémon GO). They are motivated by:

1. **The thrill of randomised discovery** — the anticipation of a pull and the satisfaction of getting a rare item.
2. **Collection completion** — the drive to fill out an inventory and see progress towards a full set.

Our secondary audience is **students and young adults with a curiosity about nature and conservation**, who find traditional learning dry but respond well to game-based experiences. For this audience, Gaia Gacha offers a way to engage with ecology topics — species, ecosystems, conservation status — in a format that feels natural and rewarding rather than academic.

The core insight behind the project is that **these two audiences overlap more than they might seem**: players motivated by collection are receptive to learning when it is wrapped in a satisfying game loop. The quiz system and ecology-themed items are designed specifically to reach this overlap — making learning feel like a natural part of playing, not a detour from it.

These insights shape our two focus areas:
- A satisfying, well-designed gacha and collection loop that keeps players engaged.
- An education-first ecology theme where every interaction — a pull, a quiz, a new item — teaches the player something real about the natural world.

## User Stories

1. As a player, I want to create an account and log in securely so that my collection is tied to my identity.
2. As a player, I want to spend Eco-Coins on gacha pulls so that I can discover new ecology items.
3. As a player, I want to see the item I received with clear visual feedback — its name, rarity, and tier — so that each pull feels rewarding.
4. As a player, I want my coin balance to update in real time after each pull so that I always know how many pulls I have left.
5. As a player, I want a home base screen after logging in so that I can navigate to different parts of the game easily.
6. As a player, I want to browse my inventory so that I can track my collection and see what I am still missing.
7. As a player, I want to answer ecology quizzes so that I can earn Eco-Coins while learning about the natural world.
8. As a player, I want a game with a polished, ecology-themed UI so that the experience feels cohesive and immersive.
9. As a player, I want to be able to earn more Eco-Coins through gameplay so that there is a reason to keep returning.

## Scope of Project

Gaia Gacha is a full-stack gacha game featuring a **Unity 6** frontend and a **Node.js/Express** backend, connected to a **Supabase** database. Players log in through the Unity client, which communicates with the backend over HTTP to authenticate users and process gacha pulls.

The project is split into two components:

- **`gaia-gacha-frontend`** — the Unity game, containing the login/register screen, main hub, gacha pull screen, quiz screen, and inventory screen.
- **`gaia-gacha-backend`** — the Express REST API, handling authentication and the gacha pull logic with coin deduction and inventory storage.

### System Architecture

```
┌─────────────────────────────┐
│     Unity 6 (Frontend)      │
│  AuthManager  GachaManager  │
│       (UnityWebRequest)      │
└────────────┬────────────────┘
             │ HTTP (JSON)
             ▼
┌─────────────────────────────┐
│  Node.js / Express (Backend) │
│  /api/auth/register          │
│  /api/auth/login             │
│  /api/gacha/pull             │
└────────────┬────────────────┘
             │ Supabase JS SDK
             ▼
┌─────────────────────────────┐
│         Supabase             │
│  Auth (JWT, user accounts)   │
│  profiles  (coins balance)   │
│  inventory (pulled items)    │
└─────────────────────────────┘
```

The Unity client never touches the database directly. All sensitive operations (coin deduction, inventory writes, auth) are handled exclusively on the Express server, which uses the Supabase service role key. The Unity client only holds a user-scoped JWT for identification.

Features are tagged as follows:

**[Proposed]** — planned for the Minimum Viable Product (MVP) by Splashdown

**[Current Progress]** — what has been implemented as of Milestone 1

**[Additional Features]** — add-ons to improve the product after the MVP is complete

---

## Features

### AUTHENTICATION SYSTEM

**[Proposed]**

Players will be able to register a new account with an email and password, and log in to access the gacha screen. Sessions are secured using JWTs (JSON Web Tokens) issued by Supabase. The Unity client holds the token in memory for the duration of the session.

The login/register screen will toggle between two modes — Sign In and Create Account — via a single panel, keeping the UI clean and minimal.

**[Current Progress]**

Authentication is fully implemented at Milestone 1. The Unity `AuthManager` sends POST requests to `/api/auth/register` and `/api/auth/login` on the Express backend. On successful login, the backend returns a JWT and a user UUID, which the Unity client stores as static properties (`AuthManager.Token` and `AuthManager.UserId`) for use by other scripts.

The `AuthUIManager` handles all UI state — toggling between login and register modes, displaying status messages, and transitioning to the main hub upon successful login.

Input validation (empty fields, minimum password length) is handled on the client before any network request is made.

---

### GACHA PULL SYSTEM

**[Proposed]**

Players will spend **10 Eco-Coins** per pull. Each pull produces one item drawn from a weighted pool of ecology-themed specimens:

| Item | Rarity | Drop Rate |
|------|--------|-----------|
| Mangrove Seed | Common | 70% |
| Coral Fragment | Rare | 25% |
| Giant Sea Turtle Shell | Legendary | 5% |

The pull result is displayed on an **item card** with the item's name, a colour-coded rarity badge, and a diamond indicator showing tier (1 diamond = Common, 2 = Rare, 3 = Legendary).

**[Current Progress]**

The gacha system is fully implemented at Milestone 1. The Unity `GachaManager` sends a POST request to `/api/gacha/pull` with the logged-in player's UUID. The backend:

1. Fetches the player's coin balance from the `profiles` table in Supabase.
2. Rejects the pull if the player has fewer than 10 coins.
3. Performs a **weighted random selection** using cumulative weights.
4. Deducts 10 coins from the player's balance.
5. Inserts the pulled item into the `inventory` table.
6. Returns the item name, rarity, and new coin balance to Unity.

The Unity client then updates the item card UI — switching from a "?" placeholder state to the revealed item state — and refreshes the Eco-Coins balance in the header.

The pull button is locked during a request to prevent duplicate submissions.

**[Additional Features]**

- Expand the item pool with more ecology specimens across biomes (rainforest, ocean, wetlands, arctic).
- Animated pull reveal (card flip, particle effects) to heighten the moment of anticipation.
- Pity system: guaranteed Rare after 10 pulls without one, guaranteed Legendary after 50.
- Multi-pull: spend 90 Eco-Coins for 10 pulls at once.

---

### INVENTORY SYSTEM

**[Proposed]**

Players will be able to view all items they have collected in a scrollable inventory panel, filtered by rarity. Each inventory entry will display the item name, rarity badge, and the date it was obtained.

**[Current Progress]**

The Supabase `inventory` table is populated on every successful pull (item name, rarity, user UUID, timestamp). The Unity inventory UI has not yet been implemented; this is planned for Milestone 2.

**[Additional Features]**

- Collection progress tracker: show how many unique items the player has found vs. the total pool.
- Item detail view: tapping an item shows a short ecology fact about that specimen.

---

### ECO-COINS EARNING LOOP

**[Proposed]**

Players will earn Eco-Coins through daily login bonuses and in-game activities, giving them a reason to return regularly.

**[Current Progress]**

Coin earning has not yet been implemented. For testing purposes, coin balances are seeded directly in the Supabase `profiles` table. This is planned for Milestone 2.

---

### MAIN HUB

**[Proposed]**

After logging in, players will land on a **Main Hub** screen that serves as the central navigation point for the game. From here, players can see their current Eco-Coins balance and navigate to the Gacha Screen, Quiz Screen, or Inventory Screen.

The hub is designed to feel like a nature research base — a home between expeditions into the natural world.

**[Current Progress]**

The main hub has not yet been implemented. Currently, login transitions directly to the gacha screen. The hub is planned for Milestone 2.

---

### QUIZ SCREEN

**[Proposed]**

The **Quiz Screen** presents players with ecology-themed multiple-choice questions. Answering correctly awards **Eco-Coins**, giving players a way to earn currency through knowledge rather than just waiting for a daily bonus.

Questions will be tied to the items in the gacha pool — for example, a question about mangroves or sea turtles — reinforcing the ecology theme and giving the collection a sense of educational purpose.

**[Current Progress]**

The quiz screen has not yet been implemented. This is planned for Milestone 2.

**[Additional Features]**

- Question difficulty tiers with higher coin rewards for harder questions.
- A daily quiz limit to balance coin economy.
- Unlockable questions based on items already collected.

---

### USER INTERFACE

**[Proposed]**

The game will have five main screens:

- **Auth Screen** — login/register panel with email and password inputs, mode toggle, and status feedback.
- **Main Hub** — home base after login; shows Eco-Coins balance and navigation to all other screens.
- **Gacha Screen** — displays the item card (default "?" state and revealed state) and the pull button.
- **Quiz Screen** — ecology multiple-choice questions that award Eco-Coins on correct answers.
- **Inventory Screen** — scrollable view of all collected items, filterable by rarity.

The visual style follows an ecology theme: muted greens, gold accents, and the Cinzel serif font to evoke a nature archive or field guide aesthetic.

**[Current Progress]**

The Auth Screen and Gacha Screen are implemented. The ecology-themed UI redesign has been applied across both, including:
- Cinzel-Regular SDF font for all headings.
- Rarity colours: muted green (Common), bright green (Rare), gold (Legendary).
- Three diamond indicators on the item card reflecting rarity tier.
- Rarity badge pill with colour-coded background.

The Main Hub, Quiz Screen, and Inventory Screen are planned for Milestone 2.

---

## Timeline and Development Plan

| MS | Tasks | Description | In-Charge | Date |
|----|-------|-------------|-----------|------|
| 1 | Backend setup | Express server, Supabase integration, CORS | Matthew | 10–15 May |
| 1 | Auth routes | `/api/auth/register` and `/api/auth/login` | Jun Han | 15–18 May |
| 1 | Gacha route | `/api/gacha/pull` with weighted random and coin deduction | Matthew | 18–22 May |
| 1 | Unity project setup | Unity 6 project, URP, folder structure | Jun Han | 10–15 May |
| 1 | Auth UI | Login/register panel, mode toggle, input validation | Jun Han | 15–22 May |
| 1 | Gacha UI | Item card, rarity diamonds, badge, pull button | Jun Han | 22–28 May |
| 1 | Unity–backend integration | `AuthManager`, `GachaManager`, UnityWebRequest | Matthew | 23–28 May |
| 1 | Ecology UI redesign | Cinzel font, colour palette, ecology-themed layout | Matthew | 28–31 May |

**Evaluation Milestone 1:**
- Working register and login flow (Unity → Express → Supabase)
- Working gacha pull (coin deduction, weighted random, inventory insert, item card reveal)
- Ecology-themed UI across auth and gacha screens
- Basic input validation on the client

---

**Evaluation Milestone 2: First Working Prototype (target: 29 June)**

| Feature | User Role | User Goal | Benefit | Acceptance Criteria |
|---------|-----------|-----------|---------|---------------------|
| Main Hub | Logged-in player | Navigate to all parts of the game from one screen | Reduces friction between features; makes the app feel complete | Hub screen loads after login; buttons navigate correctly to Gacha, Quiz, and Inventory screens |
| Inventory Screen | Logged-in player | View all items I have collected, filterable by rarity | Fulfils the collection loop — players can see their progress and what they are still missing | Fetches player's inventory from Supabase and displays item name, rarity badge, and date obtained; filter by rarity works |
| Quiz Screen | Logged-in player | Answer ecology questions to earn Eco-Coins | Provides an active coin-earning loop that rewards knowledge, not just waiting | Multiple-choice questions displayed; correct answer awards coins and updates balance in backend |
| Eco-Coins earning (daily login bonus) | Logged-in player | Receive free coins each day I log in | Gives players a reason to return daily | First login of the day triggers a coin grant; subsequent logins on the same day do not |
| Expanded item pool | Logged-in player | Pull from a larger variety of ecology items | More variety makes the collection feel richer and pulls more exciting | At least 9 items across 3 biomes available in the gacha pool |

---

**Evaluation Milestone 3: MVP (target: 27 July)**
- Pity system
- Collection progress tracker
- Item detail view with ecology facts
- Polish: animations, sound, transitions

---

**Splashdown: MVP with Add-ons (target: 26 August)**
- Multi-pull
- Leaderboard (rarest items collected)
- Full playtesting and user experience refinement

---

## Software Engineering Practices

- **Version control**: All work is managed via Git with feature branches (e.g. `feat/ui-redesign`) and pull requests merged into `main`. No direct commits to `main`.
- **Separation of concerns**: The frontend (Unity/C#) and backend (Node.js) are kept in separate directories with clearly defined responsibilities. The Unity client handles only presentation logic; all business logic and database operations live on the server.
- **Security**: Sensitive operations (coin deduction, inventory writes) are performed server-side only. The Supabase service role key is never exposed to the client. Environment variables are used for all secrets via `.env` (not committed to the repo).
- **Input validation**: Client-side validation (empty fields, password length) runs before any network request is sent. Server-side validation (player exists, sufficient coins) runs before any database write.
- **Request locking**: The gacha pull button is disabled during an in-flight request to prevent duplicate submissions and race conditions.
- **Code organisation**: Scripts are split by responsibility — `AuthManager` handles API calls, `AuthUIManager` handles UI state, `GachaManager` handles pull logic and card updates.

## Proof-of-Concept

The proof of concept demonstrates the full end-to-end flow of the core feature:

1. A player registers a new account via the Unity login screen → Express creates the user in Supabase Auth and auto-confirms the email.
2. The player logs in → Express verifies credentials via Supabase, returns a JWT and UUID → Unity stores these in memory.
3. The player taps "Pull" on the gacha screen → Unity sends a POST to `/api/gacha/pull` with the UUID → Express deducts 10 coins, runs weighted random selection, inserts the item into inventory, and returns the result → Unity updates the item card and coin balance.

This confirms that all three layers (Unity → Express → Supabase) are integrated and working together.

https://youtu.be/fgu2apnHhzM

## Work Log

https://docs.google.com/spreadsheets/d/1laB9u0eCk6dJM0JFLhfehZybj0WmniwXW8iZAgFjJ70/edit?usp=sharing
