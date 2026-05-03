using UnityEngine;

/// <summary>
/// Represents a physical object that can be picked up, held, dropped, or thrown by the player.
/// </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class HoldableItem : InteractableBase, IHoldableItem
{
    [SerializeField] private string itemId;
    public HoldableItem CurrentItem { get; private set; }
    public string ItemId => itemId;

    [Header("Holdable Settings")]
    [SerializeField] private bool isAvailableOnStart = false;

    [Header("Socket Settings")]
    [SerializeField] private Transform holdSocket;
    public Transform holdSocketTool;

    private Transform originalParent;
    private Collider physicsCollider;
    private Rigidbody itemRigidbody;

    /// <summary>
    /// Gets whether this item is currently held by the player.
    /// </summary>
    public bool IsHeld { get; set; }

    /// <summary>
    /// Gets whether this item is currently available for pickup.
    /// </summary>
    public bool IsAvailableForPickup { get; set; }

    /// <summary>
    /// Initializes item components and pickup availability.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (itemRigidbody == null)
            itemRigidbody = GetComponent<Rigidbody>();

        if (physicsCollider == null)
            physicsCollider = GetComponent<Collider>();

        originalParent = transform.parent;

        if (physicsCollider != null)
            physicsCollider.isTrigger = false;

        if (isAvailableOnStart)
            MakeAvailableForPickup();
        else
            DisablePickup();
    }

    /// <summary>
    /// Handles interaction with this holdable item.
    /// </summary>
    public override void Interact()
    {
        if (!CanInteract())
            return;

        PlayerItemHolder holder = FindFirstObjectByType<PlayerItemHolder>();

        if (holder == null)
            return;

            holder.TryPickUp(this);
    }

    /// <summary>
    /// Returns whether this item can currently be interacted with.
    /// </summary>
    public override bool CanInteract()
    {
        //Debug.Log($"CanInteract check: {name} | Available: {IsAvailableForPickup} | Held: {IsHeld}");
        if (!base.CanInteract())
            return false;

        if (!IsAvailableForPickup)
            return false;

        if (IsHeld)
            return false;

        return true;
    }

    /// <summary>
    /// Makes this item available for pickup.
    /// </summary>
    public void MakeAvailableForPickup()
    {
        IsAvailableForPickup = true;
        EnableInteraction();

        int interactableLayer = LayerMask.NameToLayer("Interactable");

        if (interactableLayer != -1)
            SetLayerRecursively(gameObject, interactableLayer);

        if (physicsCollider != null)
        {
            physicsCollider.enabled = true;
            physicsCollider.isTrigger = false;
        }
    }

    /// <summary>
    /// Makes this item unavailable for pickup.
    /// </summary>
    public void DisablePickup()
    {
        IsAvailableForPickup = false;
        DisableInteraction();
    }

    /// <summary>
    /// Picks up this item and aligns its hold socket with the player's hold point.
    /// </summary>
    public void PickUp(Transform holdPoint)
    {
        if (holdPoint == null) return;

        if (holdPoint.name != "HoldTool")
        {
            IsHeld = true;
            IsAvailableForPickup = false;
        }

            SetHighlight(false);

        if (itemRigidbody != null)
        {
            //itemRigidbody.isKinematic = true;
            //itemRigidbody.useGravity = false;
            //itemRigidbody.interpolation = RigidbodyInterpolation.None;

#if UNITY_6000_0_OR_NEWER
            itemRigidbody.linearVelocity = Vector3.zero;
#else
        itemRigidbody.velocity = Vector3.zero;
#endif
            itemRigidbody.angularVelocity = Vector3.zero;

            itemRigidbody.isKinematic = true;
            itemRigidbody.useGravity = false;
            itemRigidbody.interpolation = RigidbodyInterpolation.None;
        }

        if (physicsCollider != null)
        {
            physicsCollider.enabled = false;
        }

        if (holdSocket == null)
        {
            transform.SetParent(holdPoint, true);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            AlignSocketToHoldPoint(holdPoint);
            transform.SetParent(holdPoint, true);
        }
        if (holdPoint.name != "HoldTool")
        {
            DisableInteraction();
        }

    }

    /// <summary>
    /// Drops this item from the hold point and restores physics interaction.
    /// </summary>
    public void Drop(Vector3 force)
    {
        IsHeld = false;
        IsAvailableForPickup = true;

        transform.SetParent(null, true);

        if (physicsCollider != null)
        {
            physicsCollider.enabled = true;
        }

        if (itemRigidbody != null)
        {
            itemRigidbody.isKinematic = false;
            itemRigidbody.useGravity = true;
            itemRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            itemRigidbody.AddForce(force, ForceMode.Impulse);
        }

        EnableInteraction();
    }

    /// <summary>
    /// Aligns the hold socket transform with the provided hold point.
    /// </summary>
    private void AlignSocketToHoldPoint(Transform holdPoint)
    {
        Quaternion rotationOffset = holdPoint.rotation * Quaternion.Inverse(holdSocket.rotation);
        transform.rotation = rotationOffset * transform.rotation;

        Vector3 positionOffset = holdPoint.position - holdSocket.position;
        transform.position += positionOffset;
    }

    /// <summary>
    /// Sets the layer recursively for the object and all child objects.
    /// </summary>
    private void SetLayerRecursively(GameObject targetObject, int layer)
    {
        targetObject.layer = layer;

        foreach (Transform child in targetObject.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    public void ClearItem()
    {
        CurrentItem = null;
    }
}