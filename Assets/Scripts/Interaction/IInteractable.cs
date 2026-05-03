/// <summary>
/// Represents an object that can be interacted with by the player.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player starts looking at the interactable object.
    /// </summary>
    void OnHoverEnter();

    /// <summary>
    /// Called when the player stops looking at the interactable object.
    /// </summary>
    void OnHoverExit();

    /// <summary>
    /// Executes the interaction logic for the object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Determines whether the object can currently be interacted with.
    /// </summary>
    bool CanInteract();
}