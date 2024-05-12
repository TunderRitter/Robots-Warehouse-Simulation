using System.Text.Json;

namespace Warehouse_Simulation_Model.Persistence;

/// <summary>
/// Class that reads the configuration files.
/// </summary>
public static class ConfigReader
{
    #region Fields
    /// <summary>
    /// Sets JSON property names
    /// </summary>
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    #endregion

    #region Methods
    /// <summary>
    /// Reads config file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns><see cref="SchedulerData"/> object containing the read values.</returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static SchedulerData Read(string path)
    {
        try
        {
            string configParent = Directory.GetParent(path)?.FullName ?? throw new DirectoryNotFoundException();
            ConfigFile config = JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(path), _options);
            bool[,] map = ReadMap(Path.Combine(configParent, config.MapFile.Replace("/", "\\")));
            (int, int)[] robots = ReadCoordinates(Path.Combine(configParent, config.AgentFile.Replace("/", "\\")), map);
            (int, int)[] targets = ReadCoordinates(Path.Combine(configParent, config.TaskFile.Replace("/", "\\")), map);
            return new SchedulerData
            {
                Map = map,
                Robots = robots,
                Targets = targets,
                TeamSize = config.TeamSize,
                TasksSeen = config.NumTasksReveal,
                Strategy = config.TaskAssignmentStrategy,
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Reads map file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>A matrix where the value is <see langword="true"/> at index i, j if there is a wall, otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool[,] ReadMap(string path)
    {
        try
        {
            string FullContent = File.ReadAllText(path);
            string[] Lines = FullContent.Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries);
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

    /// <summary>
    /// Reads coordinates for robot and target positions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="map"></param>
    /// <returns>Array containing the coordinates.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static (int, int)[] ReadCoordinates(string path, bool[,] map)
    {
        try
        {
            int[] lines = Array.ConvertAll(File.ReadAllText(path).Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries), int.Parse);
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

    /// <summary>
    /// Converts coordinates.
    /// </summary>
    /// <param name="coor"></param>
    /// <param name="width"></param>
    /// <returns>A Tuple containing the x and y coordinates.</returns>
    private static (int, int) ConvertCoordinates(int coor, int width) => (coor / width, coor % width);
    #endregion
}
