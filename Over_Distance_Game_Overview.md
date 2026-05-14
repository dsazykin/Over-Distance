# Over Distance - Technical Project Overview

## Core Concept & Stack
**Over Distance** is a 2D top-down action prototype developed in **Unity 6 (6000.3.15f1)** using the **Universal Render Pipeline (URP)** with the 2D Renderer. 

### Key Technologies:
*   **Input:** Unity New Input System (Action-based).
*   **Physics:** 2D Physics engine with a custom Layer Collision Matrix for phasing.
*   **Procedural Generation:** Prefab-based room generation with camera-slide transitions and dynamic boundary clamping.
*   **Pathfinding:** Modular, per-room A* Grid-based system for decentralized navigation.
*   **Rendering:** URP 2D, Sprite Lit shaders, and multi-layered Parallax with Y-axis locking.

---

## Technical Architecture

### 1. World & Generation Systems

#### Procedural Dungeon Generator (`DungeonGenerator.cs` + `Room.cs`)
*   **Layout Algorithm:** A branching queue-based algorithm that places pre-authored Room prefabs on a virtual grid. It uses a `Dictionary<Vector2, Room>` to manage grid positions and prevent overlaps.
*   **Room Anatomy:** Every room prefab is a self-contained unit featuring its own `PathGrid`, `Tilemap` layers, and directional `Door` triggers.
*   **Exit Sealing:** Post-generation, the system iterates through all spawned rooms and instantiates `wallBlockPrefab` objects over any unconnected exits to ensure the play area is fully enclosed.

#### Room Transitions & Camera Management (`Door.cs` + `CameraFollow.cs`)
*   **Transition Logic:** `Door.cs` triggers a call to `DungeonGenerator.TransitionToRoom`. The player is teleported to a specific `spawnPoint` in the target room, calculated to be the opposite of their exit direction (e.g., exiting North teleports you to the South door's spawn).
*   **Dynamic Clamping:** The `CameraFollow` script utilizes a `SetRoomLimits` method. When entering a room, `Room.OnPlayerEnter()` provides the specific `minX/maxX` and `minY/maxY` values for that layout. This allows the camera to follow the player while preventing it from ever showing the empty "void" outside the room's walls.

#### Visual Consistency (`ParallaxBackground.cs`)
*   **Y-Axis Locking:** To prevent the horizon (sky) from moving when the player travels North/South in a top-down view, the parallax logic is locked. The background's Y-position strictly follows the camera's Y plus an initial offset.
*   **X-Axis Parallax:** Horizontal movement still applies the `parallaxSpeed` multiplier to create depth as the player moves East/West.
*   **Reset Mechanism:** During high-speed camera slides (room transitions), `ResetParallax()` is called to update the `lastCameraX` reference without moving the background, preventing the layers from "snapping" or flying off-screen.

---

### 2. Player Systems

#### Movement & Dash (`PlayerMovement.cs`)
*   **Locomotion:** Handled in `FixedUpdate` using `Rigidbody2D.MovePosition`. This ensures frame-rate independent movement that respects physics constraints.
*   **Direction Persistence:** `lastMovement` (Vector2) stores the last non-zero movement vector to determine facing direction for idle sprites and weapon hitbox placement.
*   **Dash Logic:** A Coroutine-based `PerformDash` that:
    1. Temporarily overrides normal movement logic.
    2. Switches the `SpriteRenderer` to a specific directional dash pose.
    3. Disables the `Animator` to prevent walk cycles from overriding the static dash sprite.
    4. Restores state via `UpdateAnimationState()` upon completion.

#### Combat & Hit Detection (`PlayerMovement.cs` + `WeaponDamage.cs`)
*   **Attack Execution:** The `PerformAttack` coroutine handles hitbox positioning. It rotates the `weaponHitbox` by 90, 180, or 270 degrees and offsets it by 0.7 units in the `lastMovement` direction.
*   **Hit Delivery:** `WeaponDamage.cs` uses `OnTriggerEnter2D` to pass `damageAmount` and `knockbackForce` to the target. It calculates the knockback direction using `(enemy.pos - weapon.pos).normalized`.

#### Health & Survival (`PlayerHealth.cs`)
*   **Per-Attacker Cooldowns:** Global invulnerability frames (I-Frames) have been removed. Instead, `EnemyDamage.cs` tracks its own `damageCooldown` (default: 1.0s). This allows multiple enemies to damage the player in quick succession while preventing a single enemy from dealing damage every frame.
*   **Flicker Logic:** Taking damage triggers a `HandleFlicker` coroutine. To handle rapid hits, any existing flicker coroutine is stopped before a new one starts, ensuring the sprite remains visible after the effect ends.

---

### 3. Enemy & AI Systems

#### Modular A* Pathfinding (`PathGrid.cs`, `Pathfinding.cs`, `EnemyMovement.cs`)
*   **Local Grid Scanning:** `PathGrid.cs` is attached to every room. On `Awake`, it scans its local area using `Physics2D.OverlapCircle` against an `unwalkableMask` to build a node grid.
*   **Grid Switching:** When a room transition occurs, `Room.OnPlayerEnter()` iterates through all `EnemyMovement` components in the scene and calls `UpdatePathfindingGrid(localGrid)`.
*   **Navigation:** Enemies use a custom A* implementation in `Pathfinding.cs` (calculating G, H, and F costs) to find the shortest path, following waypoints with `Rigidbody2D.MovePosition`.

#### Modular Knockback System (`EnemyHealth.cs`)
*   **Calculation:** `finalForce = weaponKnockback * (1 - enemyResistance)`. 
*   **Enemy Resistance:** A 0.0 to 1.0 slider on `EnemyHealth.cs` that scales incoming force.
*   **Execution:** `EnemyMovement.ApplyKnockback` overrides pathfinding for `knockbackDuration`, applying a `linearVelocity` burst in the hit direction.

---

## Physics & Layer Configuration

The game utilizes a **Phasing Physics Model**. This allows enemies to pass through the player's physical body while still allowing the player to collide with walls and detect damage via a dedicated trigger.

### Layer Matrix
| Layer | Description | Collides With |
| :--- | :--- | :--- |
| **Default** | General objects | World |
| **Player** | Player's physical "Feet" collider | World |
| **Enemy** | Enemy's physical body | World, Hurtbox, Obstacles |
| **World** | Walls/Environment | Player, Enemy |
| **Hurtbox** | Player's "Body" trigger | Enemy |

**Key Physics Logic:**
*   `Player` vs `Enemy` collision is **Disabled** in Physics2D settings.
*   Damage is detected when an `Enemy` layer collider enters the `Hurtbox` layer trigger.

---

## Optimization & Best Practices

### Animator Hashes
The project uses `Animator.StringToHash` to pre-calculate integer IDs for all animation states (e.g., `WalkFrontHash`). This avoids the CPU overhead of string comparisons during runtime `animator.Play()` calls.

### Component Decoupling
Systems use `GetComponentInParent` and `FindFirstObjectByType` to interact with one another, ensuring that components like `EnemyHealth` and `EnemyMovement` can be easily swapped or modified without breaking the entire AI pipeline.

---

## Directory Structure

```
Assets/
├── Scripts/                
│   ├── DungeonGenerator.cs # Procedural grid management
│   ├── Room.cs             # Room metadata & camera boundaries
│   ├── Door.cs             # Transition triggers & spawn points
│   ├── PathGrid.cs         # Room-local A* data structures
│   ├── EnemyMovement.cs    # AI navigation & knockback override
│   ├── PlayerMovement.cs   # Input, Locomotion & Dash
│   ├── PlayerHealth.cs     # Life, Death & Flicker visuals
│   └── ...
├── Animations/Daniel/      # .anim clips (Walk, Attack, etc.)
├── Sprites/Players/Daniel/ # Sprite sheets & directional poses
└── Settings/               # URP & Input System assets
```

---

## Known Limitations & Gaps
*   **Broken Attack Animations:** Animator states are currently bypassed for attacks in favor of static poses while clips are being debugged.
*   **UI/HUD:** No visual representation of health or dash cooldowns.
*   **Audio:** No sound effect or music integration.
