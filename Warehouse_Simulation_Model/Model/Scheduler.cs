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

        for (int i = 0; i < _robots.Length; i++)
        {
            AssignTask(_robots[i]);
        }

        CalculateRoutes();

        while(_targets.Count > 0 && Steps >= _to_step) ;
        {
            for (int i = 0; i < _robots.Length; i++)
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
        if (sender != null)
        {
            if (sender.GetType() == typeof(Robot)) AssignTask(sender); //megnézem hogy robot-e és null-e de még mindig rinyál
        }
    }

    public void AssignTask(Robot robot)
    {
        robot.TargetPos = null;
        if(_targets.Count > 0)
        {
            Target target = _targets.Dequeue();
            (int X, int Y) pos = target.Pos;
            robot.TargetPos = (pos.X, pos.Y);
        }
    }

    public void CalculateStep(Robot robot, int i)
    {
        (int X, int Y) pos_to = _routes[i].Peek();
        (int X, int Y) pos_from = robot.Pos;

        string move = "";

        if(pos_from.X == pos_to.X)
        {
            if(pos_from.Y == pos_to.Y - 1)
            {
                switch (robot.Direction)
                {
                    case Direction.N:
                        move = "L";
                        break;
                    case Direction.W:
                        move = "G";
                        break;
                    case Direction.S:
                        move = "R";
                        break;
                    case Direction.E:
                        move = "L";
                        break;
                }
            }
            else if(pos_from.X == pos_to.Y + 1)
            {
                switch (robot.Direction)
                {
                    case Direction.N:
                        move = "R";
                        break;
                    case Direction.W:
                        move = "L";
                        break;
                    case Direction.S:
                        move = "L";
                        break;
                    case Direction.E:
                        move = "G";
                        break;
                }
            }
        }
        else if(pos_from.Y == pos_to.Y)
        {
            if(pos_from.X == pos_to.X - 1)
            {
                switch (robot.Direction)
                {
                    case Direction.N:
                        move = "G";
                        break;
                    case Direction.W:
                        move = "R";
                        break;
                    case Direction.S:
                        move = "L";
                        break;
                    case Direction.E:
                        move = "L";
                        break;
                }
            }
            else if(pos_from.X == pos_to.X + 1)
            {
                switch (robot.Direction)
                {
                    case Direction.N:
                        move = "L";
                        break;
                    case Direction.W:
                        move = "L";
                        break;
                    case Direction.S:
                        move = "G";
                        break;
                    case Direction.E:
                        move = "R";
                        break;
                }
            }
        }

        ExecuteStep(robot, i, move);
    }

    public void ExecuteStep(Robot robot, int i, String move)
    {
        switch (move)
        {
            case "G":
                robot.Pos = _routes[i].Dequeue();
                break;
            case "L":
                TurnRobotLeft(robot);
                break;
            case "R":
                TurnRobotRight(robot);
                break;
        }
        //write to log??
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
