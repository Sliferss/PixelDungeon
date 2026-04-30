using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RoguelikeCore.Tiles
{
    /// <summary>
    /// Describes the runtime properties of a single TerrainType.
    /// Stored inside TileRegistry (a ScriptableObject), NOT inside LevelData.
    ///
    /// LevelData only stores a TerrainType per cell; the renderer looks up
    /// the matching TileDefinition at display time.  This keeps save data
    /// lean and lets you change visuals without touching saved levels.
    /// </summary>
    [Serializable]
    public class TileDefinition
    {
        [Header("Identity")]
        public TerrainType terrainType;

        [Header("Gameplay flags")]
        public bool isPassable      = false;  // characters can walk here
        public bool blocksLOS       = false;  // blocks field-of-view raycasting
        public bool isOpaque        = false;  // purely visual — no light passes through
        public bool isDestructible  = false;  // can be destroyed / mined
        public bool isSolid         = true;   // participates in physics collision (for Unity 2D if needed)

        [Header("Rendering")]
        // The TileBase Unity uses when painting this terrain onto a Tilemap.
        // Assign in the Inspector via TileRegistry.
        public TileBase unityTile;

        // Optional: a colour tint applied on top of the tile sprite.
        public Color tint = Color.white;

        // -------------------------------------------------------------------------
        // Convenience factory helpers — useful in unit tests and procedural editors
        // that don't have an Inspector.
        // -------------------------------------------------------------------------

        public static TileDefinition Floor() => new TileDefinition
        {
            terrainType    = TerrainType.Floor,
            isPassable     = true,
            blocksLOS      = false,
            isOpaque       = false,
            isDestructible = false,
            isSolid        = false,
        };

        public static TileDefinition Wall() => new TileDefinition
        {
            terrainType    = TerrainType.Wall,
            isPassable     = false,
            blocksLOS      = true,
            isOpaque       = true,
            isDestructible = false,
            isSolid        = true,
        };
    }
}
