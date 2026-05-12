# Over Distance - Technical Project Overview

## Core Concept & Stack
**Over Distance** is a 2D top-down action prototype developed in **Unity 2022.3+** using the **Universal Render Pipeline (URP)** with the 2D Renderer. 

### Key Technologies:
*   **Input:** Unity New Input System (Action-based).
*   **Physics:** 2D Physics engine with a custom Layer Collision Matrix for phasing.
*   **Rendering:** URP 2D, Sprite Lit shaders, and multi-layered Parallax.
*   **Architecture:** Component-based with a focus on decoupling through interface-like component checks (`GetComponentInParent`).

---

## Technical Architecture

### 1. Player Systems

#### Movement & Dash (`PlayerMovement.cs`)
*   **Physics:** Movement is handled in `FixedUpdate` using `Rigidbody2D.MovePosition`. This ensures frame-rate independent movement that respects the physics engine's collision constraints.
*   **Input Handling:** Uses `OnMove` and `OnDash` callbacks from the Input System.
*   **Direction Persistence:** `lastMovement` (Vector2) stores the last non-zero movement vector to determine facing direction for idles, dashes, and weapon hitbox placement.
*   **Dash Logic:** A Coroutine-based dash (`PerformDash`) that:
    1. Temporarily overrides `FixedUpdate` movement logic.
    2. Switches the `SpriteRenderer.sprite` to a directional dash pose (`dashSpriteDown`, etc.).
    3. Toggles `animator.enabled` to prevent animation states from overriding the static dash sprite.
    4. Applies a `Color.blue` tint and restores state via `UpdateAnimationState()` upon completion.

#### Health & Survival (`PlayerHealth.cs`)
*   **Damage Flow:** `TakeDamage(int)` handles health subtraction and triggers the `HandleIFrames` coroutine.
*   **Invulnerability (I-Frames):** Implemented via a boolean flag and visual flickering (`spriteRenderer.enabled` toggling).
*   **Death Hook:** Disables `PlayerMovement` and tints the character gray.

---

### 2. Enemy & Combat Systems

#### Combat Loop
1.  **Attack Trigger:** `PlayerMovement` activates a `weaponHitbox` GameObject.
2.  **Damage Detection:** `WeaponDamage.cs` (on the hitbox) uses `OnTriggerEnter2D`.
3.  **Component Resolution:** Uses `collision.GetComponentInParent<EnemyHealth>()`. This "InParent" pattern allows for complex enemy prefabs where colliders may be nested inside child objects (Hurtboxes).
4.  **Feedback:** `EnemyHealth` triggers a `FlashRed` coroutine and manages its own destruction.

#### Enemy AI (`EnemyMovement.cs`)
*   **Targeting:** Uses `FindFirstObjectByType<PlayerMovement>()` on `Start` to locate the player.
*   **Movement:** Simple normalized direction chasing using `rb.MovePosition`.

#### Contact Damage (`EnemyDamage.cs`)
*   Supports both `OnCollisionStay2D` and `OnTriggerStay2D` to allow enemies to be either physical obstacles or triggers.

---

## Physics & Layer Configuration

The game utilizes a **Phasing Physics Model**. This allows enemies to pass through the player's physical body while still allowing the player to collide with walls and detect damage.

### Layer Matrix
| Layer | Description | Collides With |
| :--- | :--- | :--- |
| **Default** | General objects | World |
| **Player** | Player's physical "Feet" collider | World |
| **Enemy** | Enemy's physical body | World, Hurtbox |
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
│   ├── EnemyHealth.cs      # Damage Reception
│   ├── WeaponDamage.cs     # Attack Delivery
│   └── ...
├── Sprites/Players/Daniel/ # Sprite sheets & Static poses
└── Settings/               # URP & Input Assets
```

---

## Known Limitations & Gaps
*   **Static Idles:** The upward idle currently uses a static sprite instead of an animation loop.
*   **AI Pathfinding:** Enemies move in a straight line; no A* or NavMesh implementation yet.
*   **UI/HUD:** No visual representation of health or combat status.
*   **Audio:** No sound effect or music integration.
