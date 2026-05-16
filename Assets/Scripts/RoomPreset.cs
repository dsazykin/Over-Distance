using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Room Preset", menuName = "Dungeon/Room Preset")]
public class RoomPreset : ScriptableObject
{
    public Room.RoomType roomType;
    public GameObject roomPrefab;
    
    [Header("Generation Settings")]
    [Tooltip("How likely this specific layout is to appear (higher = more common)")]
    public int weight = 10;
    
    [Header("Requirements")]
    public bool mustHaveNorth;
    public bool mustHaveSouth;
    public bool mustHaveEast;
    public bool mustHaveWest;
}
