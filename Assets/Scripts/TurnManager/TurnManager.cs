using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public int actionsPerTurn = 2;

    public int CurrentActions { get; private set; }

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        CurrentActions = actionsPerTurn;
        Debug.Log($"Player Turn Started. Actions: {CurrentActions}");
    }

    public bool SpendAction()
    {
        if (CurrentActions <= 0)
        {
            Debug.Log("No actions remaining.");
            return false;
        }

        CurrentActions--;
        Debug.Log($"Action spent. Remaining: {CurrentActions}");

        if (CurrentActions <= 0)
        {
            EndPlayerTurn();
        }

        return true;
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Player Turn Ended.");

        // Temporary prototype behavior:
        // Immediately start a new turn.
        StartPlayerTurn();
    }
}