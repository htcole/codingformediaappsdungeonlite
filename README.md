# 🛡️ 3D Roguelite/Dungeon Template: Student Guide

This is a template Unity Game Engine project for procedural, rogue lite dungeons as part of WVU's Coding for Media Applications course. 

It uses Unity Version 6000.3.25f1

This project is designed to help you learn core Unity development patterns, game loop design, and system architecture by building a fast-paced, replayable action game.

---

## 🎮 Game Loop Overview

Every run in a roguelite follows a classic cycle:

1. **Explore & Combat:** Fight incoming hazards, survive, and collect gold coins dropped by enemies.
2. **Shop & Invest:** Spend your gold at safe stations to buy permanent stat boosts or build-defining perks.
3. **Progress & Repeat:** Dive deeper into increasingly difficult encounters with your upgraded abilities.

---

## 🏗️ Core System Architecture

The project is built around modular, reusable scripts that talk to each other cleanly. Here are the core managers and components:

### 1. `GameManager.cs` (The Persistent Brain)

* **Design Pattern:** Singleton (`GameManager.Instance`) with `DontDestroyOnLoad`.
* **Purpose:** It tracks global meta-progression across rooms and runs—such as your total **Gold**, purchased **Max Health upgrades**, and whether you have unlocked special perks like **Vampiric Leech**. Because it persists, your stats carry over smoothly.

### 2. `PlayerHealth.cs` (Dynamic Health & Scaling)

* **Purpose:** Manages the player's current and max health.
* **Key Feature:** It calculates max health dynamically by combining a default base health with whatever extra health you've purchased at the shop. It also supports percentage-based healing (`HealPercent`), meaning healing items automatically scale up in power if your health pool grows.

### 3. `Hazard.cs` (Combat & AI)

* **Purpose:** Controls enemy movement (tracking the player) and combat interactions.
* **Key Feature:** Handles taking damage, spawning visual feedback effects, dropping gold loot, and checking if the **Vampiric Leech** perk is active to drain life from enemies on defeat.

### 4. `UpgradeStation.cs` (Distance-Based Shop Loop)

* **Purpose:** Allows the player to approach a station and spend gold to buy upgrades.
* **Key Feature:** Uses a clean **distance-based check** in `Update()` instead of physics triggers to ensure smooth, bug-free interaction. It supports multiple shop types (e.g., cheap health boosts vs. expensive build-defining life steal).

---

## ⌨️ Controls & Input System

This project uses Unity’s modern **Input System package** rather than the legacy input manager.

* **Movement:** *(Configured via your player movement action maps)*
* **Interact / Buy:** Press **`E`** when standing near an upgrade station.
* **Combat:** Attacks/interactions communicate directly with target components using safe null-checking also by pressing 'E' when near the enemy.

---

## 🚀 Key Unity Concepts to Explore in This Project

As you work with this template, pay close attention to how these patterns are implemented:

* **Singletons & Data Persistence:** How `GameManager` keeps data alive between scenes.
* **Decoupled Systems:** How `Hazard` doesn't need to know everything about the player—it simply asks for `PlayerHealth` when it needs to apply damage or steal life.
* **Modern Input Handling:** Using `Keyboard.current.eKey.wasPressedThisFrame` for responsive, device-agnostic input polling.

---

## 💡 Challenge Ideas

Want to take this project further? Try implementing any of these features:

1. **New Shop Items:** Create more upgrade types in `UpgradeStation` (like a speed boost or a dash ability).
2. **Dynamic Difficulty:** Make hazards spawn faster, deal more damage, or change as the player collects more gold, like the health dynamically upgrades over runs.
3. **UI Polish:** Bind a visual text element like the gold sample to put more of the debugging notes on the screen.
4. **Themed Set** Make your rogue lite/dungeon less generic by giving it a theme, story, and art.
5. **More Rooms:** If you look for it, there are rooms and a pattern to which they spawn. Can you make a more engaging pattern in a loop (death)? What other rooms would you add?

You do not have to use Unity for Project 2, but you will be introduced to it as part of the beginning of this unit. You may use Godot, Visual Studio, Lua, or another scripting program of choice for this section. **It may not be visual scripting.**
