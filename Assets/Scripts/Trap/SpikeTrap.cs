using UnityEngine;

public class SpikeTrap : Trap
{
    public int damage = 2;

    protected override void OnTrigger(Unit unit)
    {
        Debug.Log("Spike trap triggered!");
        unit.TakeDamage(damage);
    }
}