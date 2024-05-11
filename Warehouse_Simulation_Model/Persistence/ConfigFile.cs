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
    public string mapFile { get; set; }
	/// <summary>
	/// Property that stores the name of the agent file.
	/// </summary>
	public string agentFile { get; set; }
	/// <summary>
	/// property that stores the team size.
	/// </summary>
	public int teamSize { get; set; }
	/// <summary>
	/// Property that stores the name of the task file.
	/// </summary>
	public string taskFile { get; set; }
	/// <summary>
	/// Property that stores the number of tasks the scheduler sees.
	/// </summary>
	public int numTasksReveal { get; set; }
	/// <summary>
	/// Property that stores the task assignment strategy.
	/// </summary>
	public string taskAssignmentStrategy { get; set; }
	#endregion
}
