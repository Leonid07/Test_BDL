/// <summary>
/// Defines all repair stages in the brake disc replacement process.
/// </summary>
public enum RepairStage
{
    LiftVehicle,
    RemoveHubCap,
    UnscrewWheelBolts,
    RemoveWheel,
    UnscrewCaliperBolts,
    RemoveCaliper,
    RemoveBrakePads,
    RemoveBrakeDisc,
    TakeNewBrakeDisc,
    InstallNewBrakeDisc,
    InstallBrakePads,
    InstallCaliper,
    TightenCaliperBolts,
    InstallWheel,
    TightenWheelBolts,
    InstallHubCap,
    DownVehicle,
    Complete
}