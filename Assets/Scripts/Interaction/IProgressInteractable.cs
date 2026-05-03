/// <summary>
/// Represents an interactable object that requires progress-based interaction.
/// </summary>
public interface IProgressInteractable
{
    /// <summary>
    /// Starts the progress interaction.
    /// </summary>
    void StartProgressInteraction();

    /// <summary>
    /// Updates the progress interaction while input is held.
    /// </summary>
    void UpdateProgressInteraction(float deltaTime);

    /// <summary>
    /// Cancels the current progress interaction.
    /// </summary>
    void CancelProgressInteraction();
}