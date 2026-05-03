using UnityEngine;

/// <summary>
/// Base class for interactable objects that are available only during a specific repair stage.
/// </summary>
public abstract class StageInteractableBase : InteractableBase
{
    [Header("Stage Settings")]
    [SerializeField] protected RepairStageController repairStageController;
    [SerializeField] protected RepairStage requiredStage;

    /// <summary>
    /// Returns whether this object can be interacted with during the current repair stage.
    /// </summary>
    public override bool CanInteract()
    {
        if (!base.CanInteract())
            return false;

        if (repairStageController == null)
            return false;

        return repairStageController.IsStage(requiredStage);
    }
}