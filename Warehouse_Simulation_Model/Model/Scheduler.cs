using System.Diagnostics;
using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly List<Target> _targets;
    private readonly Log _log;
    private readonly Queue<(int, int)>[] _routes;
    private readonly ITaskAssigner _strategy;
    private readonly AStar _astar;
    private double _timeLimit;
    private readonly int _teamSize;
    private readonly int _targetsSeen;
    private bool _robotFreed;
    private Controller _controller;

    private const bool _passThrough = false;

    public Cell[,] Map { get; private set; } // Encapsulation!
    public int MaxSteps { get; set; }
    public int Step { get; private set; }
    public double TimeLimit
    {
        get { return _timeLimit; }
        set
        {
            _timeLimit = value;
        }
    }

    public event EventHandler? ChangeOccurred;


    public Scheduler(SchedulerData data)
    {
        int height = data.Map.GetLength(0);
        int width = data.Map.GetLength(1);
        Map = new Cell[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (data.Map[i, j])
                    Map[i, j] = new Wall();
                else
                    Map[i, j] = new Floor();
            }
        }
        _robots = new Robot[data.Robots.Length];
        for (int i = 0; i < data.Robots.Length; i++)
        {
            Robot robot = new(i, data.Robots[i], Direction.N);
            _robots[i] = robot;
            ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = robot;
            robot.Finished += Robot_Finished;
        }
        _targets = new List<Target>();
        foreach ((int row, int col) targetPos in data.Targets)
        {
            Target target = new(targetPos);
            _targets.Add(target);
            ((Floor)Map[target.Pos.row, target.Pos.col]).Target = target;
        }

        _log = new Log();
        WriteLogStart();

        _routes = new Queue<(int, int)>[data.Robots.Length];
        for (int i = 0; i < data.Robots.Length; i++)
        {
            _routes[i] = new Queue<(int, int)>();
        }

        _strategy = TaskAssignerFactory.Create(data.Strategy);

        _timeLimit = 1000; // !!!
        _targetsSeen = data.TasksSeen;
        _robotFreed = false;
        _controller = new Controller(new AStar(data.Map));

        MaxSteps = 10000; // !!!
        Step = 1;
    }

    public void Schedule()
    {
        DateTime startTime, endTime;
        startTime = DateTime.Now;

        AddTargets();
        AssignTasks();
        CalculateRoutes();
        ChangeOccurred?.Invoke(this, EventArgs.Empty);

        while(Step <= MaxSteps) 
        {
            if (_robotFreed)
            {
                AddTargets();
                AssignTasks();
                CalculateRoutes();
                _robotFreed = false;
            }

            for (int i = 0; i < _robots.Length; i++)
            {
                if (_routes[i].Any()) ExecuteStep(_controller.CalculateStep(_robots[i], _routes[i], _passThrough, Map, i));
                _robots[i].CheckPos();
            }

            //várjon az időlimitig, vagy ha túllépte akkor várjon megint annyit

            endTime = DateTime.Now;
            double elapsedMillisecs = (endTime - startTime).TotalMilliseconds;
            int waitTime = (int)(elapsedMillisecs / _timeLimit);

            Thread.Sleep((int)(_timeLimit * (waitTime + 1) - elapsedMillisecs));
            ChangeOccurred?.Invoke(this, EventArgs.Empty);
            
            for (int i = 0; i < waitTime; i++)
            {
                // log
            }

            Step++;
            startTime = DateTime.Now;
        }
    }

    private void AddTargets()
    {
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            _targets[i].Active = i < _targetsSeen;
            ((Floor)Map[_targets[i].Pos.row, _targets[i].Pos.col]).Target = _targets[i];
        }
    }

    private static void TurnRobotLeft(Robot robot) => robot.TurnLeft();

    private static void TurnRobotRight(Robot robot) => robot.TurnRight();

    private void Robot_Finished(object? sender, EventArgs e)
    {
        _robotFreed = true;
        if (sender is Robot robot)
        {
            _targets.RemoveAt(_targets.FindIndex(e => e.Pos == robot.Pos));
            ((Floor)Map[robot.Pos.row, robot.Pos.col]).Target = null;
        }
    }

    public void AssignTasks()
    {
        List<Robot> free = _robots.Where(e => e.TargetPos == null).ToList();
        List<Target> assignable = _targets[..Math.Min(_targetsSeen, _targets.Count)].Where(e => e.Id == null).ToList();
        _strategy.Assign(free, assignable);
    }

    public void ExecuteStep((int i, String move) step)
    {
        int i = step.i;
        String move = step.move;
        Robot robot = _robots[i];
        switch (move)
        {
            case "F":
                ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = null;
                robot.Pos = _routes[i].Dequeue();
                ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = robot;
                break;
            case "C":
                TurnRobotLeft(robot);
                break;
            case "R":
                TurnRobotRight(robot);
                break;
            case "W":
                break;
            case "WW":
                _routes[i].Dequeue();
                break;
            default:
                throw new InvalidOperationException("Invalid move");
        }
        //write to log??
    }

    public void CalculateRoutes()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            if (_robots[i].TargetPos != null && _routes[i].Count == 0)
            {
                _routes[i] = _controller.CalculateRoutes(_robots[i]);
            }
        }
    }

    public void WriteLogStart()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            Object[] data = { _robots[i].Pos.row, _robots[i].Pos.col, _robots[i].Direction.ToString() };
            _log.start.Add(data);
        }
    }

    public void WriteLog()
    {

    }

    public void AddTarget(int row, int col)
    {
        if (Map[row, col] is Wall) return;

        Target target = new((row, col));
        _targets.Add(target);
        ((Floor)Map[row, col]).Target = target;
    }
}
