using System.Text.Json;

namespace Warehouse_Simulation_Model.Persistence;


public class Log
{
    public string actionModel { get; set; }
    public string AllValid { get; set; }
    public int teamSize { get; set; }
    public List<object[]> start { get; set; }
    public int numTaskFinished { get; set; }
    public int sumOfCost { get; set; }
    public int makespan { get; set; }
    public List<string> actualPaths { get; set; }
    public List<string> plannerPaths { get; set; }
    public List<double> plannerTimes { get; set; }
    public List<object[]> errors { get; set; }
    public List<List<object[]>> events { get; set; }
    public List<int[]> tasks { get; set; }


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
}
