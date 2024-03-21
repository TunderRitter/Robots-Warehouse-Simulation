using System.Text.Json;

namespace Warehouse_Simulation_Model.Persistence;


public class Log
{
    public string actionModel {  get; set; }
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
    public List<object[]> events { get; set; }
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
            return JsonSerializer.Deserialize<Log>(File.ReadAllText(path)) ?? throw new NullReferenceException();
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
