using UnityEngine;

/// <summary>
/// Represents a wheel that can be removed and picked up during the wheel removal stage.
/// </summary>
public class WheelInteractable : StageInteractableBase
{
    [SerializeField] private HoldableItem holdableItem;
    [SerializeField] private RepairStage nextRepairStage;

    private bool isRemoved;

    /// <summary>
    /// Removes the wheel and makes it available for pickup.
    /// </summary>
    public override void Interact()
    {
        if (!CanInteract() || isRemoved)
            return;

        isRemoved = true;
        SetHighlight(false);

        if (holdableItem != null)
        {
            holdableItem.MakeAvailableForPickup();
            holdableItem.Interact();
        }

        DisableInteraction();

        repairStageController.NextStage();
    }

    /// <summary>
    /// Returns whether the wheel can currently be removed.
    /// </summary>
    public override bool CanInteract()
    {
        return base.CanInteract() && !isRemoved;
    }
}