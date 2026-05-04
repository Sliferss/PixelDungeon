using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    public bool isHidden = true;
    protected bool isTriggered = false;

    protected virtual void Start()
    {
        SetVisible(!isHidden);
    }

    public void Trigger(Unit unit)
    {
        if (isTriggered) return;

        isTriggered = true;

        OnTrigger(unit);

        Reveal();
    }

    protected abstract void OnTrigger(Unit unit);

    protected void Reveal()
    {
        isHidden = false;
        SetVisible(true);
    }

    protected void SetVisible(bool visible)
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = visible;
    }
}