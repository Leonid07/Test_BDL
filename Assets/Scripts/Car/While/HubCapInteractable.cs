using UnityEngine;

/// <summary>
/// Represents a wheel hub cap that must be removed before wheel bolts can be unscrewed.
/// </summary>
public class HubCapInteractable : StageInteractableBase
{

    private bool isRemoved;
    private HoldableItem holdableItem;

    /// <summary>
    /// Initializes component references.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        holdableItem = GetComponent<HoldableItem>();
    }

    /// <summary>
    /// Removes the hub cap and makes it available for pickup.
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
    /// Returns whether the hub cap can currently be removed.
    /// </summary>
    public override bool CanInteract()
    {
        return base.CanInteract() && !isRemoved;
    }
}