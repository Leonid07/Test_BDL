using UnityEngine;

/// <summary>
/// Controls wheel bolts progress and unlocks the wheel removal stage.
/// </summary>
public class AssemblyController : MonoBehaviour
{
    [SerializeField] private int requiredBoltCount = 5;
    [Header("References")]
    [SerializeField] private RepairStageController repairStageController;

    [SerializeField] private WheelBolt[] wheelBolts;

    private int unscrewedBoltCount;
    private int tightenedBoltCount;

    private bool tighteningPrepared;

    private void Update()
    {
        if (repairStageController == null)
            return;

        if (!tighteningPrepared && repairStageController.CurrentStage == RepairStage.TightenWheelBolts)
        {
            PrepareBoltsForTightening();
        }
    }

    public void RegisterUnscrewedBolt(WheelBolt bolt)
    {
        unscrewedBoltCount++;

        Debug.Log($"Unscrewed bolts: {unscrewedBoltCount}/{requiredBoltCount}");

        if (unscrewedBoltCount >= requiredBoltCount)
            repairStageController.NextStage();
    }

    public void RegisterTightenedBolt(WheelBolt bolt)
    {
        tightenedBoltCount++;

        Debug.Log($"Tightened bolts: {tightenedBoltCount}/{requiredBoltCount}");

        if (tightenedBoltCount >= requiredBoltCount)
            repairStageController.NextStage();
    }

    private void PrepareBoltsForTightening()
    {
        tighteningPrepared = true;
        tightenedBoltCount = 0;

        foreach (WheelBolt bolt in wheelBolts)
        {
            if (bolt != null)
                bolt.SetTighteningMode(true);
        }

        Debug.Log("Bolts prepared for tightening");
    }

    public void ResetCounters()
    {
        unscrewedBoltCount = 0;
        tightenedBoltCount = 0;
        tighteningPrepared = false;
    }
}