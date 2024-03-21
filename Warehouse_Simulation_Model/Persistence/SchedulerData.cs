namespace Warehouse_Simulation_Model.Persistence;

public readonly struct SchedulerData
{
    public bool[,] Map { get; init; }
    public (int, int)[] Robots { get; init; }
    public (int, int)[] Targets { get; init; }
    public int TeamSize { get; init; }
    public int TasksSeen { get; init; }
    public string Strategy { get; init; }
}
