using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly Queue<Target> _targets;
    private readonly double _timeLimit;
    private readonly Log _log;
    private readonly Queue<(int, int)>[] _routes;
    private readonly ITaskAssigner _method;

    private Cell[,] _map;
    public Cell[,] Map => _map; // Encapsulation!
    public int Steps { get; private set; }


    public Scheduler(int mapWidth, int mapHeight, Robot[] robots, double timeLimit, Log log, ITaskAssigner method)
    {

    }


    private List<(int, int)> AStar()
    {
        return null;
    }

    private void TurnRobotLeft(Robot robot)
    {
        robot.TurnLeft();
    }

    private void TurnRobotRight(Robot robot)
    {
        robot.TurnRight();
    }

    private void Robot_Finished(object? sender, int e)
    {

    }

    public void RemoveTarget(Target target)
    {

    }

    public void AssignTask(Robot robot)
    {

    }

    public void CalculateStep()
    {

    }

    public void ExecuteStep()
    {

    }

    public void CalculateRoutes(Robot[] robots)
    {

    }

    public void WriteLog()
    {

    }

    public void AddTarget(int x, int y)
    {

    }

}
