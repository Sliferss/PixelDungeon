using UnityEngine;
using RoguelikeCore.Tiles;

namespace RoguelikeCore.Level
{
    /// <summary>
    /// Scene-level coordinator that owns the active <see cref="LevelData"/>,
    /// drives generation, and triggers rendering.
    ///
    /// Other systems (actors, pathfinder, UI) obtain the current level through
    /// <see cref="LevelManager.Current"/>.  They must never generate or render
    /// the level themselves.
    ///
    /// Attach this to a persistent GameObject (e.g. "GameManager") in the scene.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // -------------------------------------------------------------------------
        // Singleton access — lightweight; replace with DI if the project grows.
        // -------------------------------------------------------------------------

        public static LevelManager Instance { get; private set; }

        // -------------------------------------------------------------------------
        // Inspector references
        // -------------------------------------------------------------------------

        [Header("Dependencies")]
        [SerializeField] private TileRegistry registry;
        [SerializeField] private LevelRenderer levelRenderer;

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        /// <summary>The currently loaded level.  Null before Generate is called.</summary>
        public LevelData Current { get; private set; }

        // -------------------------------------------------------------------------
        // Unity lifecycle
        // -------------------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            GenerateLevel();
        }

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Generate a new level using <see cref="SimpleRoomGenerator"/> and render it.
        /// Swap the generator here when you're ready to use a more complex one.
        /// </summary>
        public void GenerateLevel()
        {
            if (registry == null)
            {
                Debug.LogError("[LevelManager] TileRegistry is not assigned.");
                return;
            }

            var generator = new SimpleRoomGenerator(registry);
            Current = generator.Generate();

            levelRenderer?.RenderLevel(Current);

            Debug.Log($"[LevelManager] Level generated: {Current.Width}×{Current.Height}");
        }

        /// <summary>
        /// Load a previously serialized LevelData, rebuild its masks, and render it.
        /// </summary>
        public void LoadLevel(LevelData levelData)
        {
            if (levelData == null)
            {
                Debug.LogError("[LevelManager] LoadLevel called with null data.");
                return;
            }

            // Masks are not serialized — always rebuild after loading.
            levelData.BuildDerivedData(registry);
            Current = levelData;

            levelRenderer?.RenderLevel(Current);
        }

        /// <summary>
        /// Modify a single tile at runtime and update both the data layer
        /// and the visual layer in one call.
        /// </summary>
        public void SetTile(int pos, TerrainType type)
        {
            if (Current == null) return;
            Current.SetTile(pos, type, registry);
            levelRenderer?.RenderCell(Current, pos);
        }
    }
}
