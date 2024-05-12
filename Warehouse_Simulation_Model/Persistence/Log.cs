using System.Text.Json;
using System.Text.Json.Serialization;

namespace Warehouse_Simulation_Model.Persistence;

/// <summary>
/// Class that stores the data that can be saved to a file at the end of the simulation.
/// </summary>
public class Log
{
    #region Fields
    /// <summary>
    /// Sets JSON Pretty Print and property names
    /// </summary>
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    #endregion

    #region Properties
    /// <summary>
    /// Property that stores the action model.
    /// </summary>
    public string ActionModel { get; set; }
    /// <summary>
    /// Property that stores whether the simulation was collision-free.
    /// </summary>
    [JsonPropertyName("AllValid")]
    public string AllValid { get; set; }
    /// <summary>
    /// Property that stores the team size.
    /// </summary>
    public int TeamSize { get; set; }
    /// <summary>
    /// Property that stores the starting positions and directions of all robots in a list.
    /// </summary>
    public List<object[]> Start { get; set; }
    /// <summary>
    /// Property that stores the number of tasks completed.
    /// </summary>
    public int NumTaskFinished { get; set; }
    /// <summary>
    /// Property that stores the sum fo cost.
    /// </summary>
    public int SumOfCost { get; set; }
    /// <summary>
    /// Property that stores the makespan.
    /// </summary>
    public int Makespan { get; set; }
    /// <summary>
    /// Property that stores the actual paths of the robots.
    /// </summary>
    public List<string> ActualPaths { get; set; }
    /// <summary>
    /// Property that stores the planned paths of the robots.
    /// </summary>
    public List<string> PlannerPaths { get; set; }
    /// <summary>
    /// Property that stores the time each step takes.
    /// </summary>
    public List<float> PlannerTimes { get; set; }
    /// <summary>
    /// Property that stores the errors that occurred during the simulation (collisions).
    /// </summary>
    public List<object[]> Errors { get; set; }
    /// <summary>
    /// Property that stores the events that occurred during the simulation.
    /// </summary>
    public List<List<object[]>> Events { get; set; }
    /// <summary>
    /// Property that stores the tasks and their positions.
    /// </summary>
    public List<int[]> Tasks { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Log"/> class.
    /// </summary>
    public Log()
    {
        ActionModel = "";
        AllValid = "";
        TeamSize = 0;
        Start = [];
        NumTaskFinished = 0;
        SumOfCost = 0;
        Makespan = 0;
        ActualPaths = [];
        PlannerPaths = [];
        PlannerTimes = [];
        Errors = [];
        Events = [];
        Tasks = [];
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
            Log log = JsonSerializer.Deserialize<Log>(File.ReadAllText(path), _options) ?? throw new NullReferenceException();
            for (int i = 0; i < log.Start.Count; i++)
            {
                log.Start[i][0] = ((JsonElement)log.Start[i][0]).GetInt32();
                log.Start[i][1] = ((JsonElement)log.Start[i][1]).GetInt32();
                log.Start[i][2] = ((JsonElement)log.Start[i][2]).GetString()
                    ?? throw new InvalidDataException();
            }
            for (int i = 0; i < log.Errors.Count; i++)
            {
                log.Errors[i][0] = ((JsonElement)log.Errors[i][0]).GetInt32();
                log.Errors[i][1] = ((JsonElement)log.Errors[i][1]).GetInt32();
                log.Errors[i][2] = ((JsonElement)log.Errors[i][2]).GetInt32();
                log.Errors[i][3] = ((JsonElement)log.Errors[i][3]).GetString()
                    ?? throw new InvalidDataException();
            }
            for (int i = 0; i < log.Events.Count; i++)
            {
                for (int j = 0; j < log.Events[i].Count; j++)
                {
                    log.Events[i][j][0] = ((JsonElement)log.Events[i][j][0]).GetInt32();
                    log.Events[i][j][1] = ((JsonElement)log.Events[i][j][1]).GetInt32();
                    log.Events[i][j][2] = ((JsonElement)log.Events[i][j][2]).GetString()
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
            File.WriteAllText(path, JsonSerializer.Serialize(this, _options));
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion
}
