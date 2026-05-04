using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public TurnManager turnManager;
    public Unit selectedUnit;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Left button pressed");
            HandleClick();
        }
    }

    void HandleClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Debug.Log(mousePosition);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Debug.Log(worldPosition);
        worldPosition.z = 0f;

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (!hit.collider)
        {
            Debug.Log("Not hit");
            return;
        }

        Tile tile = hit.collider.GetComponent<Tile>();

        if (tile == null || !tile.IsWalkable)
        {
            Debug.Log(tile);
            if (tile != null) {
                Debug.Log(tile.IsWalkable);   
            }
            return;
        }

        if (turnManager.SpendAction())
        {
            Debug.Log("Spent action and moved");
            selectedUnit.Place(tile);
        }
    }
}