namespace Warehouse_Simulation_Model.Persistence;


public class Log
{
    public string actionModel;
    public string AllValid;
    public int teamSize;
    public List<object[]> start;
    public int numTaskFinished;
    public int sumOfCost;
    public int MakeSpan;
    public List<string> actualPaths;
    public List<string> plannerPaths;
    public List<object[]> errors;
    public List<object[]> events;
    public List<int[]> tasks;

    public Log()
    {
        actionModel = "";
        AllValid = "";
        teamSize = 0;
        start = [];
        numTaskFinished = 0;
        sumOfCost = 0;
        MakeSpan = 0;
        actualPaths = [];
        plannerPaths = [];
        errors = [];
        events = [];
        tasks = [];
    }

    public Log Read(string filename)
    {
        return new Log();
    }

    public Log Write(string filename)
    {
        return new Log();
    }
}
