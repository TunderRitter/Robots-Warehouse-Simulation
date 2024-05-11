using System.Text.Json;

namespace Warehouse_Simulation_Model.Persistence;

/// <summary>
/// Class that stores the data that can be saved to a file at the end of the simulation.
/// </summary>
public class Log
{
    #region 
    /// <summary>
    /// Property that stores the action model.
    /// </summary>
    public string actionModel { get; set; }
    /// <summary>
    /// Property that stores whether the simulation was collision-free.
    /// </summary>
    public string AllValid { get; set; }
    /// <summary>
    /// Property that stores the team size.
    /// </summary>
    public int teamSize { get; set; }
    /// <summary>
    /// Property that stores the starting positions and directions of all robots in a list.
    /// </summary>
    public List<object[]> start { get; set; }
    /// <summary>
    /// Property that stores the number of tasks completed.
    /// </summary>
    public int numTaskFinished { get; set; }
    /// <summary>
    /// Property that stores the sum fo cost.
    /// </summary>
    public int sumOfCost { get; set; }
    /// <summary>
    /// Property that stores the makespan.
    /// </summary>
    public int makespan { get; set; }
    /// <summary>
    /// Property that stores the actual paths of the robots.
    /// </summary>
    public List<string> actualPaths { get; set; }
    /// <summary>
    /// Property that stores the planned paths of the robots.
    /// </summary>
    public List<string> plannerPaths { get; set; }
    /// <summary>
    /// Property that stores the time each step takes.
    /// </summary>
    public List<float> plannerTimes { get; set; }
    /// <summary>
    /// Property that stores the errors that occurred during the simulation (collisions).
    /// </summary>
    public List<object[]> errors { get; set; }
    /// <summary>
    /// Property that stores the events that occurred during the simulation.
    /// </summary>
    public List<List<object[]>> events { get; set; }
    /// <summary>
    /// Property that stores the tasks and their positions.
    /// </summary>
    public List<int[]> tasks { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Log"/> class.
    /// </summary>
    public Log()
    {
        actionModel = "";
        AllValid = "";
        teamSize = 0;
        start = [];
        numTaskFinished = 0;
        sumOfCost = 0;
        makespan = 0;
        actualPaths = [];
        plannerPaths = [];
        plannerTimes = [];
        errors = [];
        events = [];
        tasks = [];
    }

    /// <summary>
    /// Method that reads a log file.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>A <see cref="Log"/> object.</returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static Log Read(string path)
    {
        try
        {
            Log log = JsonSerializer.Deserialize<Log>(File.ReadAllText(path)) ?? throw new NullReferenceException();
            for (int i = 0; i < log.start.Count; i++)
            {
                log.start[i][0] = ((JsonElement)log.start[i][0]).GetInt32();
                log.start[i][1] = ((JsonElement)log.start[i][1]).GetInt32();
                log.start[i][2] = ((JsonElement)log.start[i][2]).GetString()
                    ?? throw new InvalidDataException();
            }
            for (int i = 0; i < log.errors.Count; i++)
            {
                log.errors[i][0] = ((JsonElement)log.errors[i][0]).GetInt32();
                log.errors[i][1] = ((JsonElement)log.errors[i][1]).GetInt32();
                log.errors[i][2] = ((JsonElement)log.errors[i][2]).GetInt32();
                log.errors[i][3] = ((JsonElement)log.errors[i][3]).GetString()
                    ?? throw new InvalidDataException();
            }
            for (int i = 0; i < log.events.Count; i++)
            {
                for (int j = 0; j < log.events[i].Count; j++)
                {
                    log.events[i][j][0] = ((JsonElement)log.events[i][j][0]).GetInt32();
                    log.events[i][j][1] = ((JsonElement)log.events[i][j][1]).GetInt32();
                    log.events[i][j][2] = ((JsonElement)log.events[i][j][2]).GetString()
                        ?? throw new InvalidDataException();
                }
            }
            return log;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Method that writes the <see cref="Log"/> object to a file.
    /// </summary>
    /// <param name="path"></param>
    public void Write(string path)
    {
        try
        {
            File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion
}
