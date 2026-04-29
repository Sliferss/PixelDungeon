// DoorTile.cs
using UnityEngine;
using System;

/// <summary>
/// A door that can be opened or closed. Closed doors block movement and LOS.
/// Subscribe to OnDoorStateChanged to update visuals.
/// </summary>
public class DoorTile : Tile
{
    public bool IsOpen { get; private set; } = false;
    public bool IsLocked { get; private set; } = false;

    public event Action<bool> OnDoorStateChanged; // true = opened

    public DoorTile(Vector2Int position, bool startsLocked = false) : base(position)
    {
        TileID = "door";
        IsLocked = startsLocked;
        SetClosedState();
    }

    public override void OnEnter(GameObject entity)
    {
        if (!IsOpen && !IsLocked)
            Open();
    }

    public void Open()
    {
        if (IsLocked) return;
        IsOpen = true;
        SetOpenState();
        OnDoorStateChanged?.Invoke(true);
    }

    public void Close()
    {
        IsOpen = false;
        SetClosedState();
        OnDoorStateChanged?.Invoke(false);
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public void Lock()
    {
        if (!IsOpen)
            IsLocked = true;
    }

    private void SetOpenState()
    {
        IsWalkable = true;
        IsTransparent = true;
    }

    private void SetClosedState()
    {
        IsWalkable = false;
        IsTransparent = false;
    }
}