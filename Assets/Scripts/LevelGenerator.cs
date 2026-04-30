using UnityEngine;
using RoguelikeCore.Tiles;

namespace RoguelikeCore.Level
{
    // =========================================================================
    // Base class
    // =========================================================================

    /// <summary>
    /// Abstract base for all level generators.
    ///
    /// A generator's only job is to fill a LevelData with terrain and return it.
    /// It must not touch actors, items, the Tilemap, or any Unity scene object.
    ///
    /// Derive from this class to implement BSP dungeons, cellular-automata caves,
    /// hand-crafted rooms, or anything else — the contract stays the same.
    /// </summary>
    public abstract class LevelGenerator
    {
        protected TileRegistry Registry { get; }

        protected LevelGenerator(TileRegistry registry)
        {
            Registry = registry;
        }

        /// <summary>
        /// Generate and return a fully populated LevelData.
        /// Derived classes must call <see cref="LevelData.BuildDerivedData"/>
        /// before returning so masks are ready for gameplay systems.
        /// </summary>
        public abstract LevelData Generate();

        // -------------------------------------------------------------------------
        // Shared utility methods available to all generators
        // -------------------------------------------------------------------------

        /// <summary>Fill every cell in the level with one terrain type.</summary>
        protected void Fill(LevelData level, TerrainType type)
        {
            for (int i = 0; i < level.Size; i++)
                level.Map[i] = type;
        }

        /// <summary>
        /// Carve a filled rectangle (inclusive on all sides).
        /// Does not rebuild masks — call BuildDerivedData after all carving.
        /// </summary>
        protected void FillRect(LevelData level, int x, int y, int w, int h,
                                 TerrainType type)
        {
            for (int row = y; row < y + h; row++)
            for (int col = x; col < x + w; col++)
            {
                if (level.InBounds(col, row))
                    level.Map[level.PosFromXY(col, row)] = type;
            }
        }

        /// <summary>
        /// Carve only the border of a rectangle with <paramref name="wallType"/>
        /// and fill the interior with <paramref name="floorType"/>.
        /// </summary>
        protected void CarveRoom(LevelData level,
                                  int x, int y, int w, int h,
                                  TerrainType floorType, TerrainType wallType)
        {
            // Interior floor
            FillRect(level, x + 1, y + 1, w - 2, h - 2, floorType);

            // Border walls
            FillRect(level, x,         y,         w, 1, wallType); // top
            FillRect(level, x,         y + h - 1, w, 1, wallType); // bottom
            FillRect(level, x,         y,         1, h, wallType); // left
            FillRect(level, x + w - 1, y,         1, h, wallType); // right
        }
    }

    // =========================================================================
    // Concrete generator — single walled room
    // =========================================================================

    /// <summary>
    /// Creates a single rectangular room that fills the level dimensions,
    /// bordered by walls with a walkable floor interior.
    ///
    /// This is intentionally the simplest possible generator — it validates
    /// the pipeline (LevelData → generator → renderer) without any
    /// procedural complexity getting in the way.
    /// </summary>
    public class SimpleRoomGenerator : LevelGenerator
    {
        public SimpleRoomGenerator(TileRegistry registry)
            : base(registry) { }

        public override LevelData Generate()
        {
            var level = new LevelData(40, 40);

            // 1. Flood everything with Wall so nothing is accidentally passable.
            Fill(level, TerrainType.Wall);

            // 2. Carve the room: walls on the border, floor everywhere inside.
            CarveRoom(level,
                      x: 0, y: 0,
                      w: level.Width, h: level.Height,
                      floorType: TerrainType.Floor,
                      wallType:  TerrainType.Wall);

            // 3. Build passable/LOS masks from the terrain we just placed.
            level.BuildDerivedData(Registry);

            return level;
        }
    }
}
