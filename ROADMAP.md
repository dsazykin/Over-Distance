# Over Distance - Development Roadmap

This document outlines the planned trajectory for the **Over Distance** prototype, focusing on gameplay expansion, technical polish, and the required art/animation pipeline.

---

## 🚀 Immediate Milestones

### 1. UI & HUD (The "Feedback" Phase)
*   [ ] **Health Bar:** Real-time visual representation of player HP.
*   [ ] **Dash Cooldown UI:** A small icon or slider showing when the dash is ready.
*   [ ] **Damage Numbers:** Floating text when enemies (or the player) take damage.
*   [ ] **Game Over Screen:** A simple UI overlay with a "Retry" button.

### 2. Combat Expansion
*   [ ] **Knockback System:** Add physical pushback when enemies or the player are hit.
*   [ ] **Attack Combo:** A simple 3-hit combo chain for the player's weapon.
*   [ ] **Weapon Visuals:** Create a swinging sword or weapon sprite (currently using a ghost hitbox).

### 3. Enemy AI (The "Brain" Phase)
*   [ ] **A* Pathfinding:** Implement NavMesh or a simple grid-based pathfinder so enemies don't walk into walls.
*   [ ] **Enemy Types:**
    *   **Charger:** Fast, straight-line lunges.
    *   **Ranged:** Stays at a distance and fires projectiles.
*   [ ] **Spawn System:** An `EnemySpawner` that manages waves or room-clearing logic.

---

## 🎨 Art & Animation Checklist

### Required Animations (Daniel)
*   [ ] **Idle_Back:** A breathing loop for when the player faces away from the camera.
*   [ ] **Attack_Front/Side/Back:** Unique swinging animations for each direction.
*   [ ] **Hurt:** A short flinch animation for taking damage.
*   [ ] **Death:** A falling down or "poof" animation.

### Required Environment Art
*   [ ] **Destructibles:** Crates, jars, or foliage that can be broken by the weapon.
*   [ ] **Room Variants:** More tile assets for different room types (Boss room, Treasure room).
*   [ ] **Foreground Parallax:** Foreground elements (pillars, hanging vines) that move faster than the player to add depth.

---

## 🛠️ Technical Debt & Polish
*   [ ] **Input Buffer:** Allow players to "queue" a dash or attack a few frames early for smoother feel.
*   [ ] **Audio System:** Implement a `SoundManager` for footstep SFX, hit sounds, and background music.
*   [ ] **State Machine:** Refactor `PlayerMovement` into a proper State Machine (IdleState, MoveState, DashState) as complexity grows.
*   [ ] **Camera Shake:** Add a subtle screen shake when taking damage or landing a heavy hit.

---

## 🌌 Long-Term Vision
*   **Procedural Generation:** Randomly linked rooms using the current Tilemap system.
*   **Inventory/Upgrades:** A simple system for picking up items that modify `moveSpeed` or `damage`.
*   **Boss Encounter:** A large enemy with unique phases and telegraphs.
