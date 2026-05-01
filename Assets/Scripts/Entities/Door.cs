public class Door : WorldObject
{
    public bool IsOpen = false;

    public override void Interact(Actor actor)
    {
        Toggle();
    }

    private void Toggle()
    {
        IsOpen = !IsOpen;
        BlocksMovement = !IsOpen;
    }
}