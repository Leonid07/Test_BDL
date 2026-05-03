using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles camera-based interaction and progress interaction using Unity Input System.
/// </summary>
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    private IInteractable currentInteractable;
    private IProgressInteractable currentProgressInteractable;
    [SerializeField] private bool isHoldingInteraction;

    /// <summary>
    /// Gets the maximum interaction distance used by the raycast.
    /// </summary>
    public float InteractionDistance => interactionDistance;

    /// <summary>
    /// Gets the interactable layer mask used by the raycast.
    /// </summary>
    public LayerMask InteractableLayer => interactableLayer;

    /// <summary>
    /// Enables input callbacks.
    /// </summary>
    private void OnEnable()
    {
        if (interactAction == null)
            return;

        interactAction.action.Enable();
        interactAction.action.started += OnInteractStarted;
        interactAction.action.canceled += OnInteractCanceled;
    }

    /// <summary>
    /// Disables input callbacks and clears the current interaction state.
    /// </summary>
    private void OnDisable()
    {
        ClearCurrentInteractable(true);

        if (interactAction == null)
            return;

        interactAction.action.started -= OnInteractStarted;
        interactAction.action.canceled -= OnInteractCanceled;
        interactAction.action.Disable();
    }

    /// <summary>
    /// Updates hover detection and progress interaction every frame.
    /// </summary>
    private void Update()
    {
        UpdateHoverDetection();
        UpdateProgressInteraction();
    }

    /// <summary>
    /// Updates current hover target and cancels progress if the player looks away.
    /// </summary>
    private void UpdateHoverDetection()
    {
        IInteractable detectedInteractable = GetInteractableFromRaycast();

        if (detectedInteractable == currentInteractable)
            return;

        ClearCurrentInteractable(true);

        if (detectedInteractable == null)
            return;

        currentInteractable = detectedInteractable;
        currentProgressInteractable = detectedInteractable as IProgressInteractable;

        currentInteractable.OnHoverEnter();
    }

    /// <summary>
    /// Gets the first valid interactable object from the camera raycast.
    /// </summary>
    private IInteractable GetInteractableFromRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            interactionDistance,
            interactableLayer,
            triggerInteraction
        );

        if (hits.Length == 0)
            return null;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            IInteractable[] interactables = hit.collider.GetComponentsInParent<IInteractable>(true);

            foreach (IInteractable interactable in interactables)
            {
                if (interactable != null && interactable.CanInteract())
                {
                    return interactable;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Starts interaction when the interaction input is pressed.
    /// </summary>
    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (currentInteractable == null)
            return;

        if (!currentInteractable.CanInteract())
            return;

        isHoldingInteraction = true;

        if (currentProgressInteractable != null)
        {
            currentProgressInteractable.StartProgressInteraction();
            return;
        }
        currentInteractable.Interact();
    }

    /// <summary>
    /// Cancels progress interaction when the interaction input is released.
    /// </summary>
    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        CancelCurrentProgressInteraction();
    }

    /// <summary>
    /// Updates the current progress interaction while the input is held.
    /// </summary>
    private void UpdateProgressInteraction()
    {
        if (!isHoldingInteraction)
            return;

        if (currentProgressInteractable == null)
            return;

        if (interactAction != null && !interactAction.action.IsPressed())
        {
            CancelCurrentProgressInteraction();
            return;
        }

        currentProgressInteractable.UpdateProgressInteraction(Time.deltaTime);
    }

    /// <summary>
    /// Cancels the currently active progress interaction.
    /// </summary>
    private void CancelCurrentProgressInteraction()
    {
        isHoldingInteraction = false;

        if (currentProgressInteractable != null)
            currentProgressInteractable.CancelProgressInteraction();
    }

    /// <summary>
    /// Clears the current interactable object and optionally cancels progress interaction.
    /// </summary>
    private void ClearCurrentInteractable(bool cancelProgress)
    {
        if (cancelProgress)
            CancelCurrentProgressInteraction();

        if (currentInteractable != null)
            currentInteractable.OnHoverExit();

        currentInteractable = null;
        currentProgressInteractable = null;
    }
}