namespace Warehouse_Simulation_Model.Persistence;

public struct ConfigFile
{
	public string mapFile { get; set; }
	public string agentFile { get; set; }
	public int teamSize { get; set; }
	public string taskFile { get; set; }
	public int numTasksReveal { get; set; }
	public string taskAssignmentStrategy { get; set; }
}
