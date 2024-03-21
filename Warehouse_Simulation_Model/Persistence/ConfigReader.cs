using System.Text.Json;

namespace Warehouse_Simulation_Model.Persistence;


public static class ConfigReader
{
    public static SchedulerData Read(string path)
    {
        try
        {
            ConfigFile config = JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(path));
            bool[,] map = ReadMap(config.mapFile);
            (int, int)[] robots = ReadCoordinates(config.agentFile, map);
            (int, int)[] targets = ReadCoordinates(config.taskFile, map);
            return new SchedulerData
            {
                Robots = robots,
                Targets = targets,
                TasksSeen = config.numTasksReveal,
                Map = map,
            };
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
            string[] Lines = FullContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            int Height = int.Parse(Lines[1].Split(" ")[1]);
            int Width = int.Parse(Lines[2].Split(" ")[1]);
            bool[,] Map = new bool[Height, Width];
            if (Map.GetLength(1) == 0 || Map.GetLength(0) == 0) throw new ArgumentException("Map must be at least 1x1");
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

    private static (int, int)[] ReadCoordinates(string path, bool[,] map)
    {
        try
        {
            int[] lines = Array.ConvertAll(File.ReadAllText(path).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries), int.Parse);
            if (lines[0] != lines.Length - 1) throw new ArgumentException("Number of coordinates does not match");
            (int row, int col)[] coors = new (int, int)[lines[0]];
            for (int i = 0; i < coors.Length; i++)
            {
                coors[i] = ConvertCoordinates(lines[i + 1], map.GetLength(1));
                if (coors[i].row < 0
                    || coors[i].row >= map.GetLength(0)
                    || coors[i].col < 0
                    || coors[i].col >= map.GetLength(1)) throw new ArgumentException("Out of bounds");
                if (map[coors[i].row, coors[i].col]) throw new ArgumentException("Wall");
            }
            return coors;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static (int, int) ConvertCoordinates(int coor, int width) => (coor / width, coor % width);
}
