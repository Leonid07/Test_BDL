using UnityEngine;

/// <summary>
/// Base class for all interactable objects that supports outline highlighting.
/// </summary>
[RequireComponent(typeof(Outline))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Highlight Settings")]
    [SerializeField, HideInInspector] protected Outline outlineComponent;

    protected bool isInteractionEnabled = true;

    /// <summary>
    /// Disables outline when the object is initialized.
    /// </summary>
    protected virtual void Awake()
    {
        if (outlineComponent == null)
            outlineComponent = GetComponentInChildren<Outline>();

        SetHighlight(false);
    }

    /// <summary>
    /// Disables outline when the object becomes active.
    /// </summary>
    protected virtual void OnEnable()
    {
        SetHighlight(false);
    }

    /// <summary>
    /// Enables outline when the player looks at this object.
    /// </summary>
    public virtual void OnHoverEnter()
    {
        if (!CanInteract())
            return;

        SetHighlight(true);
    }

    /// <summary>
    /// Disables outline when the player stops looking at this object.
    /// </summary>
    public virtual void OnHoverExit()
    {
        SetHighlight(false);
    }

    /// <summary>
    /// Enables or disables the outline component.
    /// </summary>
    protected void SetHighlight(bool value)
    {
        if (outlineComponent != null)
            outlineComponent.enabled = value;
    }

    /// <summary>
    /// Enables interaction for this object.
    /// </summary>
    public virtual void EnableInteraction()
    {
        isInteractionEnabled = true;
    }

    /// <summary>
    /// Disables interaction for this object.
    /// </summary>
    public virtual void DisableInteraction()
    {
        isInteractionEnabled = false;
        SetHighlight(false);
    }

    /// <summary>
    /// Executes the interaction logic.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Returns whether this object can currently be interacted with.
    /// </summary>
    public virtual bool CanInteract()
    {
        return isInteractionEnabled;
    }
}