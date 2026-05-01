public class Trap : WorldObject
{
    public bool IsHidden = true;
    public bool IsTriggered = false;

    public override void OnStepped(Actor actor)
    {
        if (IsTriggered) return;

        Reveal();
        Trigger(actor);
    }

    private void Reveal()
    {
        IsHidden = false;
        // TODO: Update visuals
    }

    private void Trigger(Actor actor)
    {
        IsTriggered = true;
        // TODO: Damage / effect
    }
}