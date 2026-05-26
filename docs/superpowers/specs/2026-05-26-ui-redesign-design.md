# UI Redesign — GaiaGacha
**Date:** 2026-05-26
**Scope:** Auth panel + Gacha panel visual redesign in Unity

---

## Overview

Refactor the Unity scene's visual layer to establish a cohesive, ecology-themed interface using a dark forest green palette. Implemented via a Unity Editor setup script that rebuilds the scene programmatically.

---

## Color System

| Token | Hex | Used for |
|---|---|---|
| Background | `#081C15` | Scene/canvas background |
| Surface | `#1B4332` | Panels, cards, input fields |
| Accent | `#52B788` | Progress bars, highlights |
| Button | `#D4A373` | Primary action buttons |
| Button Text | `#1B4332` | Text on buttons |
| Text Primary | `#F1FAEE` | Headings, body text |
| Text Muted | `#95B8A0` | Placeholder text, subtitles |
| Gold | `#E9C46A` | Stars, coin icon |
| Common | `#A8B5A2` | Common rarity badge |
| Rare | `#52B788` | Rare rarity badge |
| Legendary | `#E9C46A` | Legendary rarity badge |

---

## Auth Panel Layout

Top to bottom:

1. **Logo area** — rounded square icon + "GAIAGACHA" bold title + "DISCOVER · PULL · COLLECT" spaced caps subtitle
2. **Form card** — rounded dark surface panel containing:
   - Email input field (dark surface, muted placeholder text)
   - Password input field (same styling)
   - Sign In button (full width, earth `#D4A373`, dark green text)
   - "Don't have an account? Register" muted text link below button
3. **Status text** — error/feedback text below the card

**Toggle behavior:** Clicking the Register link swaps the button to "Create Account" and the link to "Already have an account? Sign In" — toggling between login and register modes without switching panels.

---

## Gacha Panel Layout

Top to bottom:

1. **Header bar** — "GaiaGacha" title left-aligned + Eco-Coins balance with gold dot icon right-aligned
2. **Banner label** — "Nature's Collection" in muted spaced caps
3. **Item card** — dark surface card containing:
   - Stars row (1 = Common, 2 = Rare, 3 = Legendary) in gold `#E9C46A`
   - Placeholder circle (future sprite slot)
   - Item name in bold cream text
   - Rarity badge — pill-shaped, color coded per rarity
   - Default state: "?" placeholder with "Ready to discover your ecosystem" text
4. **Pull button** — full width, earth `#D4A373`, "Pull · 10 Eco-Coins"
5. **Status text** — muted feedback text below button

---

## Editor Script Architecture

**New file:** `Assets/Editor/SceneBuilder.cs`

Adds a **GaiaGacha → Build Scene** menu item to the Unity toolbar. When run it:

1. Clears the existing Canvas and UI objects from the scene
2. Creates a new Canvas with dark background
3. Builds the Auth Panel and wires up `AuthManager` + `AuthUIManager`
4. Builds the Gacha Panel with card component and wires up `GachaManager`

**Modified scripts:**
- `AuthUIManager.cs` — updated to support login/register toggle mode
- `GachaManager.cs` — updated to drive star rating and rarity badge on the card

**Unchanged scripts:**
- `AuthManager.cs`
- `AuthData.cs`
- `GachaData.cs`
