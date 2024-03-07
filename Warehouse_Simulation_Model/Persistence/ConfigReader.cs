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

    public static bool[,] ReadMap(string path)
    {
		try
		{
            string FullContent = File.ReadAllText(path);
            string[] Lines = FullContent.Split("\n");
            int Height = int.Parse(Lines[1].Split(" ")[1]);
            int Width = int.Parse(Lines[2].Split(" ")[1]);
			bool[,] Map = new bool[Height, Width];
            if (Map.GetLength(1) == 0 || Map.GetLength(0) == 0) throw new ArgumentOutOfRangeException("Map must be at least 1x1");
            if (Lines.Length - 4 != Height)
			{
				throw new ArgumentException("The number of lines in the map doesn't match the height.");
			}
			for (int i = 4; i < Lines.Length; i++)
			{
				if (Lines[i].Length != Width)
				{
					throw new ArgumentException("The length of a line doesn't match the map's width.");
				}
				for(int j = 0; j < Width; j++)
				{
					Map[i - 4, j] = !(Lines[i][j] == '.');
				}
			}
            

            bool allwall = true;
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (!Map[i, j]) allwall = false;
                }
            }

            if (allwall) throw new ArgumentException("Map can't be just walls.");


            return Map;
        }
		catch (Exception)
		{
			throw;
		}
		
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
