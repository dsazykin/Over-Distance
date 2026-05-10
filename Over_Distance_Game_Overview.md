# Over Distance - Project Overview

## Core Concept
**Over Distance** is a 2D top-down action game built in **Unity** (URP 2D renderer). It features smooth, dynamic movement, a deep multi-layered visual style (inspired by games like *Cult of the Lamb* and *Hades*), and action-oriented combat mechanics. The project targets PC and uses Unity's **New Input System** for all player input.

---

## Project Structure

```
Assets/
  Animations/          # Animation clips (Daniel_Walk_Front, Daniel_Walk_Side)
  Backgrounds/         # Parallax layer images (night1-night4.PNG)
  Scenes/              # SampleScene.unity (main playable scene)
  Scripts/             # All gameplay code (3 scripts)
    CameraFollow.cs
    ParallaxBackground.cs
    PlayerMovement.cs
  Settings/            # URP renderer & pipeline assets
  Sprites/
    Players/Daniel/
      Static/          # Idle sprites (Down, Up, Side)
      Walk_front/      # Front walk animation frames + Animator Controller (player.controller)
      Walk_side/       # Side walk animation frames
  Tiles/               # Tilemap assets — floor tiles (IMG_6933-6951) + wall tiles + DungeonPalette.prefab
```

---

## Architecture & Systems

### 1. Player (`PlayerMovement.cs`)

The single largest script in the project. Handles movement, dashing, sprite direction, and attacking.

**Movement**
* Uses `Rigidbody2D` with `rb.MovePosition` in `FixedUpdate` for physics-based, frame-rate-independent movement.
* Default `moveSpeed`: **5**.
* Input comes from the New Input System callback `OnMove(InputValue)` which writes to a `movement` vector every time input changes (not polled per-frame).
* A separate `lastMovement` vector always stores the most recent non-zero direction — used for dash direction and idle sprite selection.

**Collision**
* The player's `BoxCollider2D` is shrunk to cover **only the feet**, creating a top-down depth illusion where the character's upper body can overlap walls and obstacles.

**Sprite & Animation System**
* Three static sprites assigned in the Inspector: `spriteDown`, `spriteUp`, `spriteSide`.
* Two animation clips: `Daniel_Walk_Front` (walking toward camera) and `Daniel_Walk_Side` (walking left/right).
* The Animator Controller lives at `Sprites/Players/Daniel/Walk_front/player.controller`.
* **How it works:** The `Animator` component is toggled on/off rather than using animation states and transitions:
    * Moving horizontally: `animator.enabled = true`, plays `Daniel_Walk_Side`. `spriteRenderer.flipX` mirrors the sprite when moving left.
    * Moving down: `animator.enabled = true`, plays `Daniel_Walk_Front`.
    * Moving up: `animator.enabled = false`, static `spriteUp` is shown (no walk-up animation yet).
    * Stopped: `animator.enabled = false`, the appropriate idle sprite is set based on `lastMovement`.
* On `Start()`, the animator is disabled and the player faces down (`spriteDown`).

**Dash**
* Triggered by `OnDash` (bound to **Spacebar** / Gamepad).
* Runs as a coroutine (`PerformDash`):
    1. Sets `isDashing = true` — `FixedUpdate` switches from normal movement to `lastMovement * dashSpeed`.
    2. Tints the sprite **blue** (`spriteRenderer.color = Color.blue`) for the duration.
    3. After `dashDuration` (default **0.2s**), stops dashing and restores color to white.
    4. After `dashCooldown` (default **1s**), re-enables dashing.
* While dashing, normal movement input is ignored (`return` early in `FixedUpdate`).
* Dash direction is always `lastMovement`, so dashing while standing still dashes in the direction the player last faced.

**Combat / Attack**
* Triggered by `OnAttack` (bound to **Left Mouse Button** / Gamepad Right Trigger / Touchscreen Tap).
* Cannot attack while dashing.
* Runs as a coroutine (`PerformAttack`):
    1. Positions `weaponHitbox` (a child GameObject) **0.7 units** out from the player's center in the facing direction.
    2. Rotates the hitbox to align with the attack direction (0/90/180/-90 degrees on Z).
    3. Activates the hitbox GameObject for `attackDuration` (default **0.15s**), then deactivates it.
* Diagonal input is resolved to the dominant axis (horizontal vs vertical) before placing the hitbox.
* The `weaponHitbox` uses a `BoxCollider2D` set to **Is Trigger** — currently no `OnTriggerEnter2D` receiver exists, so hits are detected by collision but nothing responds yet.

---

### 2. Camera (`CameraFollow.cs`)

* Runs in `LateUpdate` to track the player after all movement has been applied.
* Uses `Vector3.SmoothDamp` with a configurable `smoothTime` (default **0.15s**) for a rubber-band follow feel.
* **Y-axis clamping:** When `useLimits` is enabled (default), the camera's Y position is clamped between `minY` (-5) and `maxY` (5) to prevent showing empty space above/below the room.
* No X-axis clamping currently.
* Z position is locked via `offset.z = -10` (standard for 2D cameras).

---

### 3. Parallax Background System (`ParallaxBackground.cs`)

* Each background layer is a separate GameObject with its own `ParallaxBackground` component.
* In `LateUpdate`, calculates the camera's frame-to-frame delta and moves the layer by `delta * parallaxSpeed`.
* **`parallaxSpeed` values control depth:**
    * `0` = completely static (distant sky).
    * `0.1 - 0.3` = slow drift (far mountains/stars).
    * Close to `1` = moves with the camera (near foreground).
    * Negative values = moves *against* the camera, creating a foreground overlay effect (e.g., mist, particles).
* Background images are 2048x2048 PNGs with `Wrap Mode: Repeat` and `Draw Mode: Tiled` at a very large size to prevent visible edges.
* Currently uses 4 night-themed layers (`night1.PNG` through `night4.PNG`).
* The original design doc mentions `autoScrollSpeed` for drifting elements (clouds), but this is **not yet implemented** in the current `ParallaxBackground.cs`.

---

### 4. Tilemap & Environment

* Floor and wall tiles are painted using Unity's Tilemap system.
* Tile assets are sourced from imported images (`IMG_6933` through `IMG_6951`) with a separate `Wall.asset` and `wall_placeholder_0.asset`.
* A `DungeonPalette.prefab` tile palette is set up for painting in the Tile Palette window.
* The playable area background is transparent, letting the parallax layers show through behind the room.

---

### 5. Input Configuration (`OverDistanceInput.inputactions`)

The project uses Unity's New Input System with an action map named **"Player"** containing 4 actions:

| Action | Type | Bindings |
|--------|------|----------|
| **Move** | Value (Vector2) | WASD, Arrow Keys, Gamepad Left Stick, Joystick |
| **Look** | Value (Vector2) | Mouse Delta, Gamepad Right Stick, Joystick Hat |
| **Attack** | Button | Left Mouse Button, Gamepad Right Trigger, Touchscreen Tap |
| **Dash** | Button | Spacebar |

`Look` is defined but **not currently used** by any script.

---

## Rendering

* Uses **Universal Render Pipeline (URP) 2D** with a 2D Renderer asset at `Assets/Settings/Renderer2D.asset`.
* A default Volume Profile exists at `Assets/DefaultVolumeProfile.asset` (no custom post-processing configured yet).

---

## Current State & Known Gaps

- No walk-up animation — moving up shows a static sprite.
- No damage/health system — the weapon hitbox activates but nothing receives `OnTriggerEnter2D`.
- No enemy or NPC entities.
- No UI (health bar, menus, HUD).
- No sound or music.
- `Look` input action is unused.
- `autoScrollSpeed` for parallax layers (mentioned in design) is not implemented.
- Dash visual feedback is a simple color tint — no particle effects or animation.
- Only one scene (`SampleScene`) with a single room.
- Tile assets use placeholder names (`IMG_6933`, etc.) — not renamed to descriptive names.
