// GridMovement.cs
using UnityEngine;

/// <summary>
/// Attach to any entity (player, enemy) to give it 8-directional grid movement.
/// Reads input, resolves the target tile, fires tile events.
/// </summary>
public class GridMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private bool allowDiagonal = true;

    private GridSystem<Tile> _grid;
    private Vector2Int _currentGridPos;
    private Vector3 _targetWorldPos;
    private bool _isMoving = false;

    public Vector2Int GridPosition => _currentGridPos;

    public void Init(GridSystem<Tile> grid, Vector2Int startPos)
    {
        _grid = grid;
        _currentGridPos = startPos;
        _targetWorldPos = grid.GridToWorld(startPos);
        transform.position = _targetWorldPos;
    }

    private void Update()
    {
        if (_isMoving)
        {
            SlideToTarget();
            return;
        }

        HandleInput();
    }

    // --- Input ---

    private void HandleInput()
    {
        int x = 0, y = 0;

        // Cardinal
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) y = 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) y = -1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) x = 1;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) x = -1;

        // Diagonal — numpad or hold two keys
        if (allowDiagonal)
        {
            if (Input.GetKeyDown(KeyCode.Keypad7)) { x = -1; y = 1; } // NW
            if (Input.GetKeyDown(KeyCode.Keypad9)) { x = 1; y = 1; } // NE
            if (Input.GetKeyDown(KeyCode.Keypad1)) { x = -1; y = -1; } // SW
            if (Input.GetKeyDown(KeyCode.Keypad3)) { x = 1; y = -1; } // SE
        }

        if (x == 0 && y == 0) return;

        TryMove(new Vector2Int(x, y));
    }

    // --- Movement ---

    private void TryMove(Vector2Int direction)
    {
        // Block diagonal movement through walls (no corner cutting)
        if (allowDiagonal && direction.x != 0 && direction.y != 0)
            if (!CanCutCorner(direction)) return;

        Vector2Int targetPos = _currentGridPos + direction;
        Tile targetTile = _grid.GetTile(targetPos);

        if (targetTile == null || !targetTile.IsWalkable) return;

        // Fire exit event on current tile
        _grid.GetTile(_currentGridPos)?.OnExit(gameObject);

        // Move
        _currentGridPos = targetPos;
        _targetWorldPos = _grid.GridToWorld(targetPos);
        _isMoving = true;

        // Fire enter event on new tile
        targetTile.OnEnter(gameObject);
    }

    /// <summary>
    /// Prevents clipping diagonally through two adjacent walls.
    ///   [ ][ W ]      Moving NE: checks N and E tiles are both walkable.
    ///   [ W][  ]
    /// </summary>
    private bool CanCutCorner(Vector2Int dir)
    {
        Tile horizontal = _grid.GetTile(_currentGridPos + new Vector2Int(dir.x, 0));
        Tile vertical = _grid.GetTile(_currentGridPos + new Vector2Int(0, dir.y));

        // Both adjacent tiles must be walkable to allow diagonal
        return (horizontal != null && horizontal.IsWalkable)
            && (vertical != null && vertical.IsWalkable);
    }

    // --- Smooth Slide ---

    private void SlideToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetWorldPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _targetWorldPos) < 0.001f)
        {
            transform.position = _targetWorldPos;
            _isMoving = false;
        }
    }
}