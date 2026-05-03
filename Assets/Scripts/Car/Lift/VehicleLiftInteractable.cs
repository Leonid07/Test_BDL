using UnityEngine;

/// <summary>
/// Handles vehicle lift interaction before the wheel repair process starts.
/// </summary>
public class VehicleLiftInteractable : StageInteractableBase, IProgressInteractable
{
    [Header("UI")]
    [SerializeField] private InteractionProgressView progressView;

    [Header("Settings")]
    [SerializeField] private float duration = 3f;

    [Header("Vehicle")]
    [SerializeField] private Transform vehicleRoot;
    [SerializeField] private Vector3 liftedPositionOffset = new Vector3(0f, 1.2f, 0f);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip upStart;
    [SerializeField] private AudioClip upLoop;
    [SerializeField] private AudioClip upEnd;
    [SerializeField] private AudioClip downStart;
    [SerializeField] private AudioClip downLoop;
    [SerializeField] private AudioClip downEnd;

    private float progress;
    private bool isInteracting;
    private bool isLifted;

    private Vector3 startPosition;
    private bool isLoopingStarted;

    protected override void Awake()
    {
        base.Awake();

        if (vehicleRoot != null)
            startPosition = vehicleRoot.position;

        progress = 0f;
        isLifted = false;
    }

    public override void Interact()
    {
        StartProgressInteraction();
    }

    public void StartProgressInteraction()
    {
        if (!CanInteract())
            return;

        isInteracting = true;
        isLoopingStarted = false;

        progressView?.Show();
        progressView?.SetProgress(progress);

        bool isUp = repairStageController.CurrentStage == RepairStage.LiftVehicle;
        PlaySound(isUp ? upStart : downStart, false);
    }

    public void UpdateProgressInteraction(float deltaTime)
    {
        if (!isInteracting)
            return;

        if (repairStageController == null)
            return;

        bool isLiftStage = repairStageController.CurrentStage == RepairStage.LiftVehicle;
        bool isLowerStage = repairStageController.CurrentStage == RepairStage.DownVehicle;

        if (!isLoopingStarted && !audioSource.isPlaying)
        {
            isLoopingStarted = true;
            PlaySound(isLiftStage ? upLoop : downLoop, true);
        }

        float delta = deltaTime / duration;

        if (isLiftStage)
            progress += delta;

        if (isLowerStage)
            progress -= delta;

        progress = Mathf.Clamp01(progress);

        progressView?.SetProgress(progress);

        UpdateVehiclePosition();

        if (isLiftStage && progress >= 1f)
            CompleteLift();

        if (isLowerStage && progress <= 0f)
            CompleteLower();
    }

    public void CancelProgressInteraction()
    {
        isInteracting = false;
        isLoopingStarted = false;
        progressView?.Hide();

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private void UpdateVehiclePosition()
    {
        if (vehicleRoot == null)
            return;

        vehicleRoot.position = Vector3.Lerp(
            startPosition,
            startPosition + liftedPositionOffset,
            progress
        );
    }

    private void CompleteLift()
    {
        isInteracting = false;
        isLifted = true;

        progressView?.Hide();
        SetHighlight(false);

        PlaySound(upEnd, false);

        repairStageController.NextStage();
    }

    private void CompleteLower()
    {
        isInteracting = false;
        isLifted = false;

        progressView?.Hide();
        SetHighlight(false);

        PlaySound(downEnd, false);

        repairStageController.NextStage();
    }
    private void PlaySound(AudioClip clip, bool loop)
    {
        if (audioSource == null || clip == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }
    public override bool CanInteract()
    {
        if (!isInteractionEnabled)
            return false;

        if (repairStageController == null)
            return false;

        if (repairStageController.CurrentStage == RepairStage.LiftVehicle)
            return !isLifted;

        if (repairStageController.CurrentStage == RepairStage.DownVehicle)
            return isLifted;

        return false;
    }
}