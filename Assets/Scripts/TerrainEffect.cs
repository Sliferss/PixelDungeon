// Scripts/Terrain/TerrainEffect.cs
using UnityEngine;
using RoguelikeCore.Level;

namespace RoguelikeCore.Terrain
{
    /// <summary>
    /// Base class for anything a tile does to a character.
    /// Derive from this for every terrain behaviour you need.
    /// Attach instances to a TerrainEffectRegistry ScriptableObject.
    /// </summary>
    public abstract class TerrainEffect : ScriptableObject
    {
        [Header("Trigger")]
        [Tooltip("Fire when the character first steps onto the tile.")]
        public bool triggerOnEnter = true;

        [Tooltip("Fire every turn the character remains on the tile.")]
        public bool triggerOnStay = false;

        [Tooltip("Fire when the character leaves the tile.")]
        public bool triggerOnExit = false;

        // Called by the turn system. 'actor' is whoever is standing here.
        public abstract void Apply(object actor, LevelData level, int pos);
    }
}