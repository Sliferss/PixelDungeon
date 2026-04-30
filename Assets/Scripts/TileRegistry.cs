using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeCore.Tiles
{
    /// <summary>
    /// ScriptableObject that maps every TerrainType to its TileDefinition.
    ///
    /// Create one asset in your project (Assets → Create → RoguelikeCore → Tile Registry)
    /// and assign it to LevelRenderer and any system that needs tile properties.
    ///
    /// Usage:
    ///   TileDefinition def = registry.Get(TerrainType.Wall);
    ///   bool canWalk = def.isPassable;
    /// </summary>
    [CreateAssetMenu(
        fileName = "TileRegistry",
        menuName  = "RoguelikeCore/Tile Registry")]
    public class TileRegistry : ScriptableObject
    {
        [SerializeField]
        private List<TileDefinition> definitions = new List<TileDefinition>();

        // Built at runtime; not serialized.
        private Dictionary<TerrainType, TileDefinition> _lookup;

        // -------------------------------------------------------------------------
        // Unity lifecycle
        // -------------------------------------------------------------------------

        private void OnEnable() => BuildLookup();

        private void OnValidate() => BuildLookup();

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>Returns the TileDefinition for the given terrain type,
        /// or null if none has been registered.</summary>
        public TileDefinition Get(TerrainType type)
        {
            if (_lookup == null) BuildLookup();
            return _lookup.TryGetValue(type, out var def) ? def : null;
        }

        /// <summary>Convenience: is the tile passable?  Safe to call with any int pos.</summary>
        public bool IsPassable(TerrainType type)
        {
            var def = Get(type);
            return def != null && def.isPassable;
        }

        /// <summary>Convenience: does the tile block LOS?</summary>
        public bool BlocksLOS(TerrainType type)
        {
            var def = Get(type);
            return def != null && def.blocksLOS;
        }

        // -------------------------------------------------------------------------
        // Internal
        // -------------------------------------------------------------------------

        private void BuildLookup()
        {
            _lookup = new Dictionary<TerrainType, TileDefinition>();
            foreach (var def in definitions)
            {
                if (def == null) continue;
                if (_lookup.ContainsKey(def.terrainType))
                {
                    Debug.LogWarning(
                        $"[TileRegistry] Duplicate definition for {def.terrainType}. " +
                        "The first entry wins.");
                    continue;
                }
                _lookup[def.terrainType] = def;
            }
        }

#if UNITY_EDITOR
        /// <summary>Editor helper — populate with default definitions so a new
        /// registry is usable immediately without Inspector configuration.</summary>
        [ContextMenu("Populate defaults")]
        private void PopulateDefaults()
        {
            definitions.Clear();
            definitions.Add(TileDefinition.Floor());
            definitions.Add(TileDefinition.Wall());
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
