namespace Warehouse_Simulation_Model.Persistence;

/// <summary>
/// Class that stores the data in the configuration files.
/// </summary>
public struct ConfigFile
{
    #region Properties
    /// <summary>
    /// Property that stores the name of the map file.
    /// </summary>
    public string MapFile { get; set; }
    /// <summary>
    /// Property that stores the name of the agent file.
    /// </summary>
    public string AgentFile { get; set; }
    /// <summary>
    /// property that stores the team size.
    /// </summary>
    public int TeamSize { get; set; }
    /// <summary>
    /// Property that stores the name of the task file.
    /// </summary>
    public string TaskFile { get; set; }
    /// <summary>
    /// Property that stores the number of tasks the scheduler sees.
    /// </summary>
    public int NumTasksReveal { get; set; }
    /// <summary>
    /// Property that stores the task assignment strategy.
    /// </summary>
    public string TaskAssignmentStrategy { get; set; }
    #endregion
}
