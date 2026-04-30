using UnityEngine;
using UnityEngine.Tilemaps;
using RoguelikeCore.Tiles;

namespace RoguelikeCore.Level
{
    /// <summary>
    /// Reads a <see cref="LevelData"/> and paints it onto Unity Tilemaps.
    ///
    /// This is the only class allowed to touch UnityEngine.Tilemaps.
    /// All gameplay logic queries LevelData directly — never the Tilemap.
    ///
    /// Setup in the Inspector
    /// ──────────────────────
    ///   1. Create a Grid GameObject in the scene.
    ///   2. Add two Tilemap children: one for "Floor" layer, one for "Walls".
    ///      (Or use a single Tilemap if you prefer — assign only FloorTilemap.)
    ///   3. Add this component to the Grid (or any persistent GameObject).
    ///   4. Assign FloorTilemap, WallTilemap, and Registry in the Inspector.
    ///
    /// Alternatively, use a single Tilemap and assign it to FloorTilemap;
    /// leave WallTilemap null — the renderer will fall back to one layer.
    /// </summary>
    public class LevelRenderer : MonoBehaviour
    {
        [Header("Tilemaps")]
        [Tooltip("Tilemap used for passable floor tiles.")]
        [SerializeField] private Tilemap floorTilemap;

        [Tooltip("Tilemap used for impassable wall tiles (can be null to use floorTilemap).")]
        [SerializeField] private Tilemap wallTilemap;

        [Header("Data")]
        [Tooltip("The TileRegistry that maps TerrainTypes to TileBases.")]
        [SerializeField] private TileRegistry registry;

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Clear any existing tiles and repaint the entire level from scratch.
        /// Call this after generating or loading a level.
        /// </summary>
        public void RenderLevel(LevelData level)
        {
            if (level == null)
            {
                Debug.LogError("[LevelRenderer] RenderLevel called with null LevelData.");
                return;
            }

            ClearAll();
            PaintAll(level);
        }

        /// <summary>
        /// Repaint a single cell — use after <see cref="LevelData.SetTile"/> to
        /// reflect a tile change without repainting the whole map.
        /// </summary>
        public void RenderCell(LevelData level, int pos)
        {
            if (level == null || !level.InBounds(pos)) return;

            var coords = level.CoordsFromPos(pos);
            PaintCell(level.Map[pos], coords.x, coords.y);
        }

        // -------------------------------------------------------------------------
        // Internal
        // -------------------------------------------------------------------------

        private void ClearAll()
        {
            floorTilemap?.ClearAllTiles();
            wallTilemap?.ClearAllTiles();
        }

        private void PaintAll(LevelData level)
        {
            for (int pos = 0; pos < level.Size; pos++)
            {
                var coords = level.CoordsFromPos(pos);
                PaintCell(level.Map[pos], coords.x, coords.y);
            }
        }

        private void PaintCell(TerrainType type, int x, int y)
        {
            if (registry == null) return;

            var def = registry.Get(type);
            if (def == null || def.unityTile == null) return;

            // Route to the appropriate Tilemap layer.
            // Fall back to floorTilemap if wallTilemap is unassigned.
            Tilemap target;
            if (!def.isPassable && wallTilemap != null)
                target = wallTilemap;
            else
                target = floorTilemap;

            if (target == null) return;

            var cellPos = new Vector3Int(x, y, 0);
            target.SetTile(cellPos, def.unityTile);

            if (def.tint != Color.white)
                target.SetColor(cellPos, def.tint);
        }
    }
}
