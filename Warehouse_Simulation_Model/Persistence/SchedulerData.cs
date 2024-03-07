namespace Warehouse_Simulation_Model.Persistence;

public readonly struct SchedulerData
{
    public (int, int)[] Robots { get; init; }
    public (int, int)[] Targets { get; init; }
    public int TasksSeen { get; init; }
    public bool[,] Map { get; init; }
}
