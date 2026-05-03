using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles picking up, holding, dropping, and throwing items for the player.
/// </summary>
public class PlayerItemHolder : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private InputActionReference throwAction;

    [Header("Hold Settings")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform holdPointTool;
    [SerializeField] private Transform toolWorkingPoint;
    [SerializeField] private float dropForce = 1.5f;
    [SerializeField] private float throwForce = 5f;

    private HoldableItem currentItem;
    private ToolItem currentTool;
    //private ToolItem currentTool;
    public HoldableItem CurrentItem => currentItem;
    public ToolItem CurrentTool => currentTool;

    public Transform ToolHoldPoint => holdPointTool;
    public Transform ToolWorkingPoint => toolWorkingPoint;

    //public ITool CurrentTool => currentItem as ITool;

    /// <summary>
    /// Gets whether the player is currently holding an item.
    /// </summary>
    public bool HasItem => currentItem != null;
    public bool HasTool => currentTool != null;

    /// <summary>
    /// Gets whether the player is currently holding a tool of the requested type.
    /// </summary>
    //public bool HasTool(ToolType toolType)
    //{
    //    ITool tool = CurrentTool;
    //    return tool != null && tool.ToolType == toolType;
    //}

    /// <summary>
    /// Enables item holder input actions.
    /// </summary>
    private void OnEnable()
    {
        if (dropAction != null)
        {
            dropAction.action.Enable();
            dropAction.action.performed += OnDropPerformed;
        }

        if (throwAction != null)
        {
            throwAction.action.Enable();
            throwAction.action.performed += OnThrowPerformed;
        }
    }

    /// <summary>
    /// Disables item holder input actions.
    /// </summary>
    private void OnDisable()
    {
        if (dropAction != null)
        {
            dropAction.action.performed -= OnDropPerformed;
            dropAction.action.Disable();
        }

        if (throwAction != null)
        {
            throwAction.action.performed -= OnThrowPerformed;
            throwAction.action.Disable();
        }
    }

    /// <summary>
    /// Attempts to pick up the specified item if the player's hands are empty.
    /// </summary>
    public bool TryPickUp(HoldableItem item)
    {
        if (item == null) return false;

        // Если это инструмент
        if (item is ToolItem tool)
        {
            if (currentTool != null) return false;

            currentTool = tool;
            currentTool.PickUp(holdPointTool != null ? holdPointTool : holdPoint);
            return true;
        }

        if (currentItem != null) return false;
        if (!item.IsAvailableForPickup) return false;

        currentItem = item;
        currentItem.PickUp(holdPoint);
        return true;
    }

    /// <summary>
    /// Drops the currently held item near the player.
    /// </summary>
    public void DropCurrentItem()
    {
        if (currentItem == null)
            return;

        Vector3 force = transform.forward * dropForce;

        currentItem.Drop(force);
        currentItem = null;
    }

    /// <summary>
    /// Throws the currently held item forward.
    /// </summary>
    public void ThrowCurrentItem()
    {
        if (currentItem == null)
            return;

        Vector3 force = transform.forward * throwForce;

        currentItem.Drop(force);
        currentItem = null;
    }

    /// <summary>
    /// Handles item drop input action.
    /// </summary>
    private void OnDropPerformed(InputAction.CallbackContext context)
    {
        DropCurrentItem();
    }

    /// <summary>
    /// Handles item throw input action.
    /// </summary>
    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        ThrowCurrentItem();
    }

    /// <summary>
    /// Clears the currently held item reference without dropping it.
    /// </summary>
    public void ClearItem()
    {
        currentItem = null;
    }
}
