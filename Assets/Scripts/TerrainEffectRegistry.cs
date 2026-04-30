// Scripts/Terrain/TerrainEffectRegistry.cs
using System.Collections.Generic;
using UnityEngine;
using RoguelikeCore.Tiles;

namespace RoguelikeCore.Terrain
{
    [CreateAssetMenu(menuName = "RoguelikeCore/Terrain Effect Registry")]
    public class TerrainEffectRegistry : ScriptableObject
    {
        [System.Serializable]
        private struct Entry
        {
            public TerrainType terrainType;
            public TerrainEffect effect;
        }

        [SerializeField] private List<Entry> entries = new();

        private Dictionary<TerrainType, TerrainEffect> _lookup;

        private void OnEnable() => BuildLookup();
        private void OnValidate() => BuildLookup();

        public TerrainEffect Get(TerrainType type)
        {
            if (_lookup == null) BuildLookup();
            return _lookup.TryGetValue(type, out var e) ? e : null;
        }

        public bool HasEffect(TerrainType type) => Get(type) != null;

        private void BuildLookup()
        {
            _lookup = new();
            foreach (var entry in entries)
                _lookup[entry.terrainType] = entry.effect;
        }
    }
}