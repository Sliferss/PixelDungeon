using System;
using UnityEngine;
using RoguelikeCore.Tiles;

namespace RoguelikeCore.Level
{
    /// <summary>
    /// Pure data model for one dungeon level.
    ///
    /// Holds the terrain map and all derived boolean masks that gameplay
    /// systems query every frame.  Contains NO MonoBehaviour, NO Unity
    /// Tilemap references, and NO actor state — those live in separate layers.
    ///
    /// Fully serializable: Unity's JsonUtility, BinaryFormatter, or any custom
    /// serializer can round-trip this class without modification.
    ///
    /// Architecture contract
    /// ─────────────────────
    ///   • LevelData  = raw data only (this file)
    ///   • LevelRenderer = reads LevelData, drives Tilemaps  (separate)
    ///   • LevelGenerator = writes into a LevelData           (separate)
    ///   • Actors / pathfinder = read LevelData arrays        (separate)
    /// </summary>
    [Serializable]
    public class LevelData
    {
        // =====================================================================
        // Dimensions
        // =====================================================================

        [SerializeField] private int _width;
        [SerializeField] private int _height;

        public int Width  => _width;
        public int Height => _height;
        public int Size   => _width * _height;

        // =====================================================================
        // Terrain map
        // =====================================================================

        /// <summary>
        /// Flat 1-D array of terrain types.  Index with <see cref="PosFromXY"/>.
        /// Using byte-backed TerrainType keeps saved data compact.
        /// </summary>
        [SerializeField] private TerrainType[] _map;

        public TerrainType[] Map => _map;

        // =====================================================================
        // Derived boolean masks
        // =====================================================================
        // These are rebuilt from _map whenever the level is loaded or a tile
        // is changed.  They are NOT serialized — they are always reconstructed.
        // Marking [NonSerialized] ensures JsonUtility / BinaryFormatter skips them.

        /// <summary>True where a character can walk.</summary>
        [NonSerialized] public bool[] Passable;

        /// <summary>True where a tile blocks line-of-sight raycasting.</summary>
        [NonSerialized] public bool[] LosBlocking;

        /// <summary>True where the hero's current field-of-view reaches.</summary>
        [NonSerialized] public bool[] HeroFOV;

        /// <summary>True for every cell the hero has ever seen (fog of war memory).</summary>
        [NonSerialized] public bool[] Visited;

        // =====================================================================
        // Pre-computed neighbour offset arrays
        // =====================================================================
        // Computed once when the level is built.  Stored here so the pathfinder
        // and any other system can share them without re-allocating.

        /// <summary>4-directional (cardinal) neighbour offsets.</summary>
        [NonSerialized] public int[] Neighbours4;

        /// <summary>8-directional (including diagonal) neighbour offsets.</summary>
        [NonSerialized] public int[] Neighbours8;

        /// <summary>9-cell block (self + 8 neighbours) — for area effects.</summary>
        [NonSerialized] public int[] Neighbours9;

        // =====================================================================
        // Construction
        // =====================================================================

        /// <summary>
        /// Create an empty level filled with <see cref="TerrainType.Empty"/>.
        /// Call <see cref="SetTile"/> or fill <see cref="Map"/> directly,
        /// then call <see cref="BuildDerivedData"/> with a TileRegistry.
        /// </summary>
        public LevelData(int width, int height)
        {
            if (width  <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width  = width;
            _height = height;
            _map    = new TerrainType[width * height];

            AllocateMasks();
            BuildNeighbourOffsets();
        }

        // =====================================================================
        // Position helpers
        // =====================================================================

        /// <summary>Convert 2-D grid coordinates to a flat array index.</summary>
        public int PosFromXY(int x, int y) => x + y * _width;

        /// <summary>Extract the X component from a flat index.</summary>
        public int XFromPos(int pos) => pos % _width;

        /// <summary>Extract the Y component from a flat index.</summary>
        public int YFromPos(int pos) => pos / _width;

        /// <summary>Convert a flat index to a Vector2Int for Unity interop.</summary>
        public Vector2Int CoordsFromPos(int pos) =>
            new Vector2Int(XFromPos(pos), YFromPos(pos));

        /// <summary>Convert a Vector2Int to a flat index.</summary>
        public int PosFromCoords(Vector2Int coords) =>
            PosFromXY(coords.x, coords.y);

        /// <summary>Returns true if the position is within map bounds.</summary>
        public bool InBounds(int pos) => pos >= 0 && pos < Size;

        /// <summary>Returns true if the 2-D coordinates are within map bounds.</summary>
        public bool InBounds(int x, int y) =>
            x >= 0 && x < _width && y >= 0 && y < _height;

        // =====================================================================
        // Distance
        // =====================================================================

        /// <summary>
        /// Chebyshev (chessboard) distance between two flat positions.
        /// Diagonal movement costs the same as cardinal — matches SPD's model.
        /// </summary>
        public int Distance(int posA, int posB)
        {
            int ax = XFromPos(posA), ay = YFromPos(posA);
            int bx = XFromPos(posB), by = YFromPos(posB);
            return Mathf.Max(Mathf.Abs(ax - bx), Mathf.Abs(ay - by));
        }

        /// <summary>
        /// True Euclidean distance — use for effects that ignore the movement grid
        /// (e.g. splash radius, audio falloff).
        /// </summary>
        public float TrueDistance(int posA, int posB)
        {
            int ax = XFromPos(posA), ay = YFromPos(posA);
            int bx = XFromPos(posB), by = YFromPos(posB);
            return Mathf.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
        }

        /// <summary>Two positions are adjacent (including diagonal) when Chebyshev distance == 1.</summary>
        public bool Adjacent(int posA, int posB) => Distance(posA, posB) == 1;

        // =====================================================================
        // Tile mutation
        // =====================================================================

        /// <summary>
        /// Set a terrain type and immediately rebuild that cell's mask entries.
        /// Prefer this over writing to Map[] directly so masks stay consistent.
        /// </summary>
        public void SetTile(int pos, TerrainType type, TileRegistry registry)
        {
            if (!InBounds(pos)) return;

            _map[pos] = type;

            if (registry != null)
            {
                var def = registry.Get(type);
                Passable[pos]    = def != null && def.isPassable;
                LosBlocking[pos] = def != null && def.blocksLOS;
            }
        }

        /// <summary>Convenience overload using 2-D coordinates.</summary>
        public void SetTile(int x, int y, TerrainType type, TileRegistry registry) =>
            SetTile(PosFromXY(x, y), type, registry);

        // =====================================================================
        // Derived data rebuild
        // =====================================================================

        /// <summary>
        /// Rebuild all derived masks from the current map contents.
        /// Must be called:
        ///   • After initial level generation.
        ///   • After loading a saved level (masks are not serialized).
        ///   • After bulk tile changes (use SetTile for single-tile changes).
        /// </summary>
        public void BuildDerivedData(TileRegistry registry)
        {
            AllocateMasks();  // re-alloc in case dimensions changed
            BuildNeighbourOffsets();

            if (registry == null)
            {
                Debug.LogWarning("[LevelData] BuildDerivedData called with null registry. " +
                                 "Masks will remain empty.");
                return;
            }

            for (int pos = 0; pos < Size; pos++)
            {
                var def = registry.Get(_map[pos]);
                Passable[pos]    = def != null && def.isPassable;
                LosBlocking[pos] = def != null && def.blocksLOS;
                // HeroFOV and Visited stay false until a visibility pass runs.
            }
        }

        // =====================================================================
        // Internal helpers
        // =====================================================================

        private void AllocateMasks()
        {
            int n = Size;
            Passable    = new bool[n];
            LosBlocking = new bool[n];
            HeroFOV     = new bool[n];
            Visited     = new bool[n];
        }

        private void BuildNeighbourOffsets()
        {
            int w = _width;

            Neighbours4 = new int[]
            {
                -w,      // North
                +1,      // East
                +w,      // South
                -1,      // West
            };

            Neighbours8 = new int[]
            {
                -w - 1, -w, -w + 1,   // NW  N  NE
                    -1,         +1,   // W      E
                +w - 1, +w, +w + 1,   // SW  S  SE
            };

            // Same as Neighbours8 with self (offset 0) inserted in the centre.
            Neighbours9 = new int[]
            {
                -w - 1, -w, -w + 1,
                    -1,   0,     +1,
                +w - 1, +w, +w + 1,
            };
        }
    }
}
