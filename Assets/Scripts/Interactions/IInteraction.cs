

public interface IInteraction
{
    string InteractionName { get; }
    string InteractionDescription { get; }

    void Interact();
}