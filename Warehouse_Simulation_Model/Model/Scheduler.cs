using Warehouse_Simulation_Model.Persistence;
using System.Timers;

namespace Warehouse_Simulation_Model.Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly Queue<Target> _targets;
    private readonly double _timeLimit;
    private readonly Log _log;
    private readonly Queue<(int, int)>[] _routes;
    private readonly ITaskAssigner _method;
    private readonly AStar _astar;
    private readonly int _to_step;

    private Cell[,] _map;
    public Cell[,] Map => _map; // Encapsulation!
    public int Steps { get; private set; }


    public Scheduler(int mapWidth, int mapHeight, Robot[] robots, double timeLimit, Log log, ITaskAssigner method)
    {
        Schedule();
    }

    private void Schedule()
    {
        //System.Timers.Timer timer = new System.Timers.Timer();

        for (int i = 0; i < _robots.Count; i++)
        {
            AssignTask(_robots[i]);
        }

        CalculateRoutes();

        while(_targets.Count > 0 && Steps >= _to_step) ;
        {
            for (int i = 0; i < _robots.Count; i++)
            {
                CalculateStep(_robots[i], i);
            }
            //várjon az időlimitig, vagy ha túllépte akkor várjon megint annyit
        }

        //System.Threading.Thread.Sleep(1000);
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
        AssignTask(sender);
    }

    public void AssignTask(Robot robot)
    {
        robot.TargetPos = null;
        if(_targets.Count > 0)
        {
            (int X, int Y) pos = _targets.Dequeue();
            robot.TargetPos.X = pos.X;
            robot.TargetPos.Y = pos.Y;
        }
    }

    public void CalculateStep(Robot robot, int i)
    {
        (int X, int Y) pos_to = _routes[i][0];
        (int X, int Y) pos_from = robot.Pos;

        if(pos_from.X == pos_to.X)
        {
            if(pos_from.Y == pos_to.Y - 1)
            {
                switch (robot.Direction)
                {
                    case Direction.N:
                        //TurnRobotLeft(robot);
                    case Direction.W:
                        //odaléphet, vele szembe van
                    case Direction.S:
                        //TurnRobotRight(robot);
                    case Direction.E:
                        //TurnRobotLeft(robot);
                }
            }
        }
    }

    public void ExecuteStep()
    {

    }

    public void CalculateRoutes()
    {
        for (int i=0; i<_robots.Length; i++)
        {
            if (_robots[i].TargetPos != null && _routes[i].Count == 0)
            {
                _routes[i] = _astar.AStarSearch(_robots[i]);
            }
        }
    }

    public void WriteLog()
    {

    }

    public void AddTarget(int x, int y)
    {
        _targets.Enqueue(new Target((x, y)));
    }

}
