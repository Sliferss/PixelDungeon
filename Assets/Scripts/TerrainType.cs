using System;

namespace RoguelikeCore.Tiles
{
    /// <summary>
    /// Defines every terrain type the map array can hold.
    /// Add new entries here — LevelData and TileRegistry will pick them up
    /// automatically as long as you also register a TileDefinition for each.
    ///
    /// Values are explicit so serialized maps stay stable across refactors.
    /// Never remove or renumber existing values; mark them Obsolete instead.
    /// </summary>
    [Serializable]
    public enum TerrainType : byte
    {
        Empty       = 0,    // uninitialized / void — outside map bounds
        Floor       = 1,    // walkable ground
        Wall        = 2,    // impassable solid tile

        // --- reserve space for future terrain groups ---
        // Door        = 10,
        // DoorOpen    = 11,
        // Chasm       = 20,
        // Water       = 21,
        // Grass       = 22,
        // Trap        = 30,
        // SecretTrap  = 31,
        // Stairs      = 40,
        // StairsUp    = 41,
    }
}
