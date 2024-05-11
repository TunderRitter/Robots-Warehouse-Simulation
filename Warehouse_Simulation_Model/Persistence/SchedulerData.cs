namespace Warehouse_Simulation_Model.Persistence;

/// <summary>
/// Class that stores the data needed to run the simulation.
/// </summary>
public readonly struct SchedulerData
{
    #region Properties
    /// <summary>
    /// Property that stores the map.
    /// </summary>
    public bool[,] Map { get; init; }
    /// <summary>
    /// Property that stores the robots' positions.
    /// </summary>
    public (int, int)[] Robots { get; init; }
    /// <summary>
    /// Property that stores the targets' positions.
    /// </summary>
    public (int, int)[] Targets { get; init; }
    /// <summary>
    /// Property that stores the team size.
    /// </summary>
    public int TeamSize { get; init; }
    /// <summary>
    /// Property that stores the number of tasks seen by the scheduler.
    /// </summary>
    public int TasksSeen { get; init; }
    /// <summary>
    /// Property that stores the strategy used to assign tasks to robots.
    /// </summary>
    public string Strategy { get; init; }
    #endregion
}
