using UnityEngine;

/// <summary>
/// Represents a wheel bolt that can be unscrewed and then picked up.
/// </summary>
public class WheelBolt : StageInteractableBase, IProgressInteractable
{
    [Header("References")]
    [SerializeField] private AssemblyController wheelAssemblyController;
    [SerializeField] private InteractionProgressView progressView;
    [SerializeField] private Transform movableVisual;
    [SerializeField] private HoldableItem holdableItem;

    [Header("Mode")]
    [SerializeField] private bool isTighteningMode;

    [Header("Tool Requirement")]
    [SerializeField] private bool requireTool = true;
    [SerializeField] private ToolType requiredTool = ToolType.Wrench;
    [SerializeField] private float toolTransitionSpeed = 5f;

    [Header("Dynamometric Wrench Animation")]
    [SerializeField] private float strokeAngle = 45f;
    [SerializeField] private float pullSpeed = 1.5f;
    [SerializeField] private float returnSpeedMultiplier = 3f;

    [Header("Install Settings")]
    [SerializeField] private string requiredItemId = "bolt";
    [SerializeField] private Transform installPoint;

    [Header("Settings")]
    [SerializeField] private float totalDuration = 5f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float moveOffset = 0.01f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private Vector3 moveLocalAxis = Vector3.right;
    [SerializeField] private Vector3 rotateLocalAxis = Vector3.right;

    private float currentProgress;
    private float toolTransitionProgress;
    private bool isInteracting;
    private bool isCompleted;
    private bool isInstalled;
    private float strokeTimer;
    private bool isPullingStroke = true;
    private Quaternion initialWorkingRotation;
    private PlayerItemHolder playerItemHolder;

    private Vector3 initialVisualLocalPosition;

    protected override void Awake()
    {
        base.Awake();

        if (movableVisual == null)
            movableVisual = transform;

        initialVisualLocalPosition = movableVisual.localPosition;
        initialWorkingRotation = Quaternion.identity;
        SetTighteningMode(isTighteningMode);
    }
    private void Update()
    {
        if (!isInteracting && toolTransitionProgress > 0)
        {
            toolTransitionProgress -= Time.deltaTime * toolTransitionSpeed;
            toolTransitionProgress = Mathf.Max(0, toolTransitionProgress);

            PlayerItemHolder holder = GetPlayerItemHolder();
            if (holder != null && holder.CurrentTool != null)
            {
                AnimateToolToWorkingPoint(holder.CurrentTool, holder.ToolHoldPoint, holder.ToolWorkingPoint, toolTransitionProgress);

                if (toolTransitionProgress <= 0)
                {
                    ResetToolParent(holder);
                }
            }
        }
    }
    public void SetTighteningMode(bool value)
    {
        isTighteningMode = value;

        isCompleted = false;
        isInteracting = false;

        if (isTighteningMode)
        {
            currentProgress = 1f;
            isInstalled = false;
        }
        else
        {
            currentProgress = 0f;
            isInstalled = true;
        }

        UpdateVisualMovement();
        EnableInteraction();
    }

    public override void Interact()
    {
        StartProgressInteraction();
    }

    public void StartProgressInteraction()
    {
        if (!CanInteract() || isCompleted) return;

        PlayerItemHolder holder = FindFirstObjectByType<PlayerItemHolder>();
        if (holder == null || holder.CurrentTool == null || holder.CurrentTool.ToolType != requiredTool) return;

        if (isTighteningMode && !isInstalled)
        {
            if (!TryInsertBoltFromHand()) return;
        }

        isInteracting = true;
        isPullingStroke = true;
        strokeTimer = 0f;
    }

    public void UpdateProgressInteraction(float deltaTime)
    {
        if (!isInteracting || isCompleted) return;

        PlayerItemHolder holder = GetPlayerItemHolder();
        if (holder == null || holder.CurrentTool == null || holder.CurrentTool.ToolType != requiredTool)
        {
            CancelProgressInteraction();
            return;
        }

        ToolItem tool = holder.CurrentTool;

        if (toolTransitionProgress < 1f)
        {
            toolTransitionProgress += deltaTime * toolTransitionSpeed;
            toolTransitionProgress = Mathf.Min(1, toolTransitionProgress);
            AnimateToolToWorkingPoint(tool, holder.ToolHoldPoint, holder.ToolWorkingPoint, toolTransitionProgress);

            if (toolTransitionProgress < 1f) return;

            tool.transform.localRotation = Quaternion.identity;
            initialWorkingRotation = Quaternion.identity;
            progressView?.Show();
        }

        float currentWrenchVisualAngle = 0f;

        if (isPullingStroke)
        {
            // if (strokeTimer == 0) PlaySound(effortSound);

            strokeTimer += deltaTime * pullSpeed;
            float tStroke = Mathf.Clamp01(strokeTimer);
            currentWrenchVisualAngle = Mathf.Lerp(0, strokeAngle, tStroke);

            float delta = deltaTime / totalDuration;
            if (isTighteningMode) currentProgress -= delta;
            else currentProgress += delta;

            UpdateVisualMovement();
            UpdateVisualRotation(deltaTime);

            if (strokeTimer >= 1f)
            {
                isPullingStroke = false;
                strokeTimer = 0f;
            }
        }
        else
        {
            // if (strokeTimer == 0) PlaySound(ratchetSound);

            strokeTimer += deltaTime * pullSpeed * returnSpeedMultiplier;
            float tStroke = Mathf.Clamp01(strokeTimer);
            currentWrenchVisualAngle = Mathf.Lerp(strokeAngle, 0, tStroke);

            if (strokeTimer >= 1f)
            {
                isPullingStroke = true;
                strokeTimer = 0f;
            }
        }

        tool.transform.localRotation = Quaternion.Euler(0, currentWrenchVisualAngle, 0);

        currentProgress = Mathf.Clamp01(currentProgress);
        progressView?.SetProgress(currentProgress);

        if (isPullingStroke)
        {
            if (!isTighteningMode && currentProgress >= 1f) CompleteUnscrew();
            if (isTighteningMode && currentProgress <= 0f) CompleteTighten();
        }
    }

    private void AnimateToolToWorkingPoint(ToolItem tool, Transform from, Transform to, float t)
    {
        if (from == null || to == null) return;

        tool.transform.position = Vector3.Lerp(from.position, to.position, t);

        tool.transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, t);

        if (t >= 1f)
        {
            tool.transform.localRotation = Quaternion.identity;
            initialWorkingRotation = Quaternion.identity;
        }
    }

    public void CancelProgressInteraction()
    {
        isInteracting = false;
        progressView?.Hide();
    }

    public override bool CanInteract()
    {
        if (!base.CanInteract())
            return false;

        if (isCompleted)
            return false;

        if (RequiresToolForCurrentAction() && !HasRequiredToolInHand())
            return false;

        return true;
    }

    public bool IsToolValid(ITool tool)
    {
        if (!requireTool)
            return true;

        return tool != null && tool.ToolType == requiredTool;
    }

    private bool RequiresToolForCurrentAction()
    {
        if (!requireTool)
            return false;

        if (!isTighteningMode)
            return true;

        return isInstalled;
    }

    private bool HasRequiredToolInHand()
    {
        PlayerItemHolder holder = GetPlayerItemHolder();

        if (holder == null)
            return false;

        ITool tool = holder.CurrentTool;

        if (tool == null)
            return false;

        return tool.ToolType == requiredTool;
    }

    private bool TryInsertBoltFromHand()
    {
        if (installPoint == null)
        {
            Debug.LogError($"{name}: InstallPoint is not assigned");
            return false;
        }

        PlayerItemHolder holder = GetPlayerItemHolder();

        if (holder == null || holder.CurrentItem == null)
        {
            Debug.Log("No bolt in hand");
            return false;
        }

        HoldableItem item = holder.CurrentItem;

        if (item.ItemId != requiredItemId)
        {
            Debug.Log($"Wrong item. Need: {requiredItemId}, current: {item.ItemId}");
            return false;
        }

        holder.ClearItem();

        item.transform.SetParent(installPoint, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;

        item.DisablePickup();

        holdableItem = item;
        movableVisual = item.transform;

        initialVisualLocalPosition = movableVisual.localPosition;
        currentProgress = 1f;
        isInstalled = true;

        UpdateVisualMovement();

        Debug.Log("Bolt inserted into slot");

        return true;
    }

    private void CompleteUnscrew()
    {
        isCompleted = true;
        isInteracting = false;
        progressView?.Hide();

        PlayerItemHolder holder = FindFirstObjectByType<PlayerItemHolder>();

        if (holdableItem != null)
        {
            holdableItem.MakeAvailableForPickup();
            //PlayerItemHolder holder = FindFirstObjectByType<PlayerItemHolder>();
            if (holder != null)
            {
                if (holder.HasItem) holder.DropCurrentItem();
                holder.TryPickUp(holdableItem);
            }
        }

        DisableInteraction();
        wheelAssemblyController?.RegisterUnscrewedBolt(this);
    }

    private void CompleteTighten()
    {
        isCompleted = true;
        isInteracting = false;
        progressView?.Hide();

        SetHighlight(false);
        DisableInteraction();
        wheelAssemblyController?.RegisterTightenedBolt(this);
    }
    private void ResetToolParent(PlayerItemHolder holder)
    {
        if (holder.CurrentTool != null)
        {
            holder.CurrentTool.transform.SetParent(holder.ToolHoldPoint);
            holder.CurrentTool.transform.localPosition = Vector3.zero;
            holder.CurrentTool.transform.localRotation = Quaternion.identity;
            initialWorkingRotation = Quaternion.identity;
        }
    }

    private void UpdateVisualMovement()
    {
        if (movableVisual == null)
            return;

        Vector3 target = initialVisualLocalPosition + moveLocalAxis.normalized * moveOffset;

        movableVisual.localPosition = Vector3.Lerp(
            initialVisualLocalPosition,
            target,
            currentProgress
        );
    }

    private void UpdateVisualRotation(float deltaTime)
    {
        if (movableVisual == null)
            return;

        float direction = isTighteningMode ? -1f : 1f;

        movableVisual.Rotate(
            rotateLocalAxis.normalized,
            rotationSpeed * direction * deltaTime,
            Space.Self
        );
    }

    private PlayerItemHolder GetPlayerItemHolder()
    {
        if (playerItemHolder == null)
            playerItemHolder = FindFirstObjectByType<PlayerItemHolder>();

        return playerItemHolder;
    }
}
