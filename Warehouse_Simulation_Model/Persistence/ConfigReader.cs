using System.Text.Json;

namespace Warehouse_Simulation_Model.Persistence;


public static class ConfigReader
{
    public static void Read(string path)
    {
		try
		{
			ConfigFile config = JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(path));
            ReadMap(config.mapFile);
            ReadRobots(config.agentFile);
            ReadTargets(config.taskFile);
		}
		catch (Exception)
		{
			throw;
		}
	}

    public static void ReadMap(string path)
    {

    }

	private static void ReadRobots(string path)
	{
		List<(int, int)> robots = [];
	}

	private static void ReadTargets(string path)
	{

	}

	private static (int, int) ConvertCoordinates(int coor, int width) => (coor / width, coor % width);
}
