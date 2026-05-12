# Over Distance - Technical Project Overview

## Core Concept & Stack
**Over Distance** is a 2D top-down action prototype developed in **Unity 6 (6000.3.15f1)** using the **Universal Render Pipeline (URP)** with the 2D Renderer. 

### Key Technologies:
*   **Input:** Unity New Input System (Action-based).
*   **Physics:** 2D Physics engine with a custom Layer Collision Matrix for phasing.
*   **Pathfinding:** Custom A* Grid-based system for obstacle avoidance.
*   **Rendering:** URP 2D, Sprite Lit shaders, and multi-layered Parallax.
*   **Architecture:** Component-based with a focus on decoupling through interface-like component checks (`GetComponentInParent`).

---

## Technical Architecture

### 1. Player Systems

#### Movement & Dash (`PlayerMovement.cs`)
*   **Physics:** Movement is handled in `FixedUpdate` using `Rigidbody2D.MovePosition`. This ensures frame-rate independent movement that respects the physics engine's collision constraints.
*   **Input Handling:** Uses `OnMove` and `OnDash` callbacks from the Input System.
*   **Movement Lock:** Movement and walking animations are locked while `isAttacking` or `isDashing` is true.
*   **Direction Persistence:** `lastMovement` (Vector2) stores the last non-zero movement vector to determine facing direction for idles, dashes, and weapon hitbox placement.
*   **Dash Logic:** A Coroutine-based dash (`PerformDash`) that:
    1. Temporarily overrides `FixedUpdate` movement logic.
    2. Switches the `SpriteRenderer.sprite` to a directional dash pose (`dashSpriteDown`, etc.).
    3. Toggles `animator.enabled` to prevent animation states from overriding the static dash sprite.
    4. Restores state via `UpdateAnimationState()` upon completion.

#### Combat (`PlayerMovement.cs` + `WeaponDamage.cs`)
*   **Attack Execution:** `PerformAttack` coroutine handles the combat sequence:
    1. Locks movement and disables the animator (current implementation uses static poses due to broken animations).
    2. Rotates and positions the `weaponHitbox` based on `lastMovement`.
    3. Activates the hitbox trigger for a defined `attackDuration`.
    4. Restores the animator and movement state on completion.
*   **Hit Detection:** `WeaponDamage.cs` passes `damageAmount`, `knockbackForce`, and the weapon's `transform.position` to the target's `EnemyHealth` component.

#### Health & Survival (`PlayerHealth.cs`)
*   **Damage Flow:** `TakeDamage(int)` handles health subtraction and triggers the `HandleIFrames` coroutine.
*   **Invulnerability (I-Frames):** Implemented via a boolean flag and visual flickering (`spriteRenderer.enabled` toggling).
*   **Death Hook:** Disables `PlayerMovement` and tints the character gray.

---

### 2. Enemy & AI Systems

#### A* Pathfinding (`PathGrid.cs`, `Pathfinding.cs`, `EnemyMovement.cs`)
*   **Grid Management:** `PathGrid.cs` generates a virtual 2D grid over the playable area. Nodes are marked as unwalkable based on a `Physics2D.OverlapCircle` check against an `unwalkableMask`.
*   **Algorithm:** A custom A* implementation in `Pathfinding.cs` calculates the shortest path between the enemy and the player using G, H, and F costs.
*   **Navigation:** `EnemyMovement.cs` periodically updates its path (every 0.2s) and follows waypoints sequentially using `Rigidbody2D.MovePosition`.

#### Modular Knockback System
*   **Weapon Strength:** Each weapon defines its own `knockbackForce` in `WeaponDamage.cs`.
*   **Enemy Resistance:** `EnemyHealth.cs` features a `knockbackResistance` slider (0 to 1). 
*   **Calculation:** `finalForce = weaponKnockback * (1 - enemyResistance)`.
*   **Execution:** `EnemyMovement.ApplyKnockback` triggers a `KnockbackRoutine` that temporarily overrides pathfinding logic with a `Rigidbody2D.linearVelocity` burst.

#### Contact Damage (`EnemyDamage.cs`)
*   Supports both `OnCollisionStay2D` and `OnTriggerStay2D`.
*   **Self-Damage Protection:** `OnTriggerStay2D` explicitly ignores trigger colliders (like the player's weapon hitbox) to ensure enemies only damage the player's main body.

---

## Physics & Layer Configuration

The game utilizes a **Phasing Physics Model**. This allows enemies to pass through the player's physical body while still allowing the player to collide with walls and detect damage.

### Layer Matrix
| Layer | Description | Collides With |
| :--- | :--- | :--- |
| **Default** | General objects | World |
| **Player** | Player's physical "Feet" collider | World |
| **Enemy** | Enemy's physical body | World, Hurtbox, Obstacles |
| **World** | Walls/Environment | Player, Enemy |
| **Hurtbox** | Player's "Body" trigger | Enemy |

**Key Physics Logic:**
*   `Player` vs `Enemy` is **Disabled** in the Physics2D settings.
*   Damage is detected when an `Enemy` layer collider enters the `Hurtbox` layer trigger.

---

## Optimization & Best Practices

### Animator Hashes
To avoid the overhead of string-based lookups in `animator.Play()`, the project uses pre-computed integer hashes:
```csharp
private static readonly int WalkFrontHash = Animator.StringToHash("Walk_Front");
```
This reduces CPU usage and prevents runtime errors caused by typos in animation state names.

### Coroutine Management
Visual feedback systems (Dash, I-Frames, Hit Flashing) are handled via Coroutines to keep the `Update` loops clean and focused on gameplay logic.

---

## Directory Structure

```
Assets/
├── Animations/Daniel/      # .anim clips (Walk_Back, Walk_Front, Walk_Side)
├── Backgrounds/            # Parallax assets
├── Scripts/                # Gameplay Logic
│   ├── PlayerMovement.cs   # Input & Locomotion
│   ├── PlayerHealth.cs     # Life/Death Logic
│   ├── Node.cs             # A* Data Structure
│   ├── PathGrid.cs         # Grid Generation
│   ├── Pathfinding.cs      # A* Algorithm
│   ├── EnemyMovement.cs    # AI Navigation & Knockback
│   ├── EnemyHealth.cs      # Damage & Resistance
│   ├── WeaponDamage.cs     # Attack & Knockback Delivery
│   └── ...
├── Sprites/Players/Daniel/ # Sprite sheets & Static poses
└── Settings/               # URP & Input Assets
```

---

## Known Limitations & Gaps
*   **Broken Attack Animations:** The attack animator states are currently bypassed in favor of static poses while the `.anim` files are being debugged.
*   **UI/HUD:** No visual representation of health or combat status.
*   **Audio:** No sound effect or music integration.
