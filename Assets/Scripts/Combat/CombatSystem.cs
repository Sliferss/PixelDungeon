using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public bool Attack(Unit attacker, Unit target, TurnManager turnManager)
    {
        if (!turnManager.SpendAction())
            return false;

        // Damage logic here
        return true;
    }
}
