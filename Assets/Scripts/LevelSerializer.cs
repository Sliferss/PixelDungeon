using System;
using System.IO;
using UnityEngine;

namespace RoguelikeCore.Level
{
    /// <summary>
    /// Handles serialization of <see cref="LevelData"/> to and from JSON.
    ///
    /// Uses Unity's JsonUtility for maximum compatibility (no external packages).
    /// Swap for Newtonsoft.Json or a binary format here if you need more control.
    ///
    /// Note: Only the terrain map array is serialized.  All boolean masks
    /// (Passable, LosBlocking, HeroFOV, Visited) are marked [NonSerialized]
    /// and must be rebuilt via <see cref="LevelData.BuildDerivedData"/> after loading.
    /// </summary>
    public static class LevelSerializer
    {
        private static string SaveDirectory =>
            Path.Combine(Application.persistentDataPath, "Saves");

        // -------------------------------------------------------------------------
        // Save
        // -------------------------------------------------------------------------

        /// <summary>Serialize a level to JSON and write it to disk.</summary>
        /// <param name="level">The level to save.</param>
        /// <param name="fileName">Filename without extension, e.g. "level_01".</param>
        public static void Save(LevelData level, string fileName = "level")
        {
            if (level == null) throw new ArgumentNullException(nameof(level));

            Directory.CreateDirectory(SaveDirectory);
            string path = Path.Combine(SaveDirectory, fileName + ".json");
            string json = JsonUtility.ToJson(level, prettyPrint: true);
            File.WriteAllText(path, json);

            Debug.Log($"[LevelSerializer] Saved to {path}");
        }

        // -------------------------------------------------------------------------
        // Load
        // -------------------------------------------------------------------------

        /// <summary>
        /// Load a level from disk.  Returns null if the file does not exist.
        /// Remember to call <see cref="LevelData.BuildDerivedData"/> on the result.
        /// </summary>
        public static LevelData Load(string fileName = "level")
        {
            string path = Path.Combine(SaveDirectory, fileName + ".json");

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[LevelSerializer] Save file not found: {path}");
                return null;
            }

            string json = File.ReadAllText(path);
            var level = JsonUtility.FromJson<LevelData>(json);

            Debug.Log($"[LevelSerializer] Loaded from {path}");
            return level;
        }

        // -------------------------------------------------------------------------
        // To / from string (useful for network transfer or embedded saves)
        // -------------------------------------------------------------------------

        public static string ToJson(LevelData level) =>
            JsonUtility.ToJson(level, prettyPrint: false);

        public static LevelData FromJson(string json) =>
            JsonUtility.FromJson<LevelData>(json);
    }
}
