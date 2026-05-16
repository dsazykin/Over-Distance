# Guide: Setting Up Procedural Room Prefabs

Follow these steps to convert your tilemaps into modular rooms that work with the `DungeonGenerator`, `CameraFollow`, and `Pathfinding` systems.

---

## 1. Room Hierarchy Structure
Every room should be a **Prefab** with this exact structure:

*   **Room_Base** (GameObject with `Room.cs` and `PathGrid.cs`)
    *   **GridCenter** (Empty GameObject at the dead center of the room)
    *   **Tilemaps** (Grid with Floors, Walls, and Props)
    *   **Exits** (Folder)
        *   **Door_North** (GameObject with `Door.cs`, Direction: North)
            *   **SpawnPoint** (Empty GameObject 1-2 units *inside* the door)
        *   **Door_South** ... (Repeat for all 4 directions)
    *   **Enemies** (Folder for enemies that belong to this room)

---

## 2. Configuring the `Room` Script
Attach `Room.cs` to the root of the prefab and fill in these fields:

1.  **Exits:** Check the boxes (`hasNorth`, `hasEast`, etc.) based on which doors are physically built into this room layout.
2.  **Camera Constraints:**
    *   Find the **Center** of your room.
    *   Calculate how far the camera can move left/right (`minX`, `maxX`) and up/down (`minY`, `maxY`) before it shows the "void" outside the walls.
    *   *Tip:* If your room is 20 units wide and your camera view is 18 units wide, the camera can only move 1 unit to the left and 1 to the right.
3.  **Doors:** Drag the specific Door GameObjects from the hierarchy into the `North Door`, `South Door`, etc., slots.
4.  **Local Grid:** Drag the `PathGrid` component (attached to this same object) into this slot.

---

## 3. Configuring the `PathGrid`
Attach `PathGrid.cs` to the root of the prefab:

1.  **Grid World Size:** Set this to the internal floor area of your room (e.g., `X: 18, Y: 13`).
2.  **Node Radius:** Use `0.25` or `0.5` (smaller radius = more precise but heavier on CPU).
3.  **Unwalkable Mask:** Ensure this is set to your **World** or **Obstacles** layer.
4.  **Verification:** In the Editor, you should see white/red boxes (Gizmos) appearing over your room. If red boxes cover your floor, your `Unwalkable Mask` is wrong!

---

## 4. Configuring `Door` Triggers
Each door needs:

1.  **Direction:** Set correctly (North, South, East, West).
2.  **Parent Room:** Drag the root **Room_Base** into this slot.
3.  **Spawn Point:** This is the most important part. When a player enters the **South** door of the next room, they will teleport to **this** door's `Spawn Point`. 
    *   Place it just inside the doorway so they don't immediately trigger the door again.
4.  **Collider:** Add a `BoxCollider2D` set to **Is Trigger**. Size it so it spans the entire doorway.

---

## 5. Adding to the Generator
1.  Open your **DungeonManager** GameObject in the scene.
2.  Assign your "Start Room" prefab to the `Start Room Prefab` slot.
3.  Add all your other room variants to the `Room Prefabs` list.
4.  **Room Spacing:** This must match your physical room size. If your rooms are 20x15 units, set `Room Spacing` to `X: 20, Y: 15`.

---

## 6. Pro-Tip: Wall Blocks
In the `DungeonGenerator`, assign a simple 1x1 Wall Prefab to the `Wall Block Prefab` slot. The generator will automatically spawn these to block off any exits that didn't find a neighbor, preventing the player from walking into the void!
