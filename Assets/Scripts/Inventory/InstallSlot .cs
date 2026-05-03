using UnityEngine;

/// <summary>
/// Represents a slot where a specific item can be installed.
/// </summary>
public class InstallSlot : StageInteractableBase
{
    [SerializeField] private Transform installPoint;
    [SerializeField] private string requiredItemId;

    /// <summary>
    /// Installs the currently held item if it matches the required type.
    /// </summary>
    public override void Interact()
    {
        if (!CanInteract())
            return;

        PlayerItemHolder holder = FindFirstObjectByType<PlayerItemHolder>();

        if (holder == null || holder.CurrentItem == null)
            return;

        HoldableItem item = holder.CurrentItem;

        if (item.ItemId != requiredItemId)
        {
            Debug.Log("Wrong item");
            return;
        }

        InstallItem(item, holder);

        repairStageController.NextStage();
    }

    /// <summary>
    /// Places the item into the install point.
    /// </summary>
    private void InstallItem(HoldableItem item, PlayerItemHolder holder)
    {
        holder.ClearItem();

        item.transform.SetParent(installPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        item.DisablePickup();
        item.EnableInteraction();
    }
}