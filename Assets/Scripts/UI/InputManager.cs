using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles mouse click movement using Unity's New Input System.
/// </summary>
public sealed class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleLeftClick();
    }

    private void HandleLeftClick()
    {
        if (mainCamera == null)
        {
            Debug.LogError("[InputManager] No camera assigned.");
            return;
        }

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;

        GridPosition target =
            GridManager.Instance.WorldToGrid(worldPosition);

        PathfindingSystem.Instance.RequestPath(target);
    }
}