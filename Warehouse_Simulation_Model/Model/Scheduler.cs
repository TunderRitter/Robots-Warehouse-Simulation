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

    private const bool _passThrough = true;

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
        _routes = new Queue<(int, int)>[data.Robots.Length];
        for (int i = 0; i < data.Robots.Length; i++)
        {
            _routes[i] = new Queue<(int, int)>();
        }

        _strategy = TaskAssignerFactory.Create(data.Strategy);
        _astar = new AStar(data.Map);

        _timeLimit = 1000; // !!!
        _targetsSeen = data.TasksSeen;
        _robotFreed = false;

        MaxSteps = 10000; // !!!
        Step = 1;
    }

    public void Schedule()
    {
        Debug.WriteLine(MaxSteps);
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
                if (_routes[i].Any()) CalculateStep(i);
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
            Debug.WriteLine(Step);
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

    public void CalculateStep(int i)
    {
        Robot robot = _robots[i];
        (int row, int col) posTo = _routes[i].Peek();
        (int row, int col) posFrom = robot.Pos;

        string move = "";

        if (posFrom.row == posTo.row)
        {
            if (posFrom.col - 1 == posTo.col)
            {
                move = robot.Direction switch
                {
                    Direction.N => "C",
                    Direction.E => "R",
                    Direction.S => "R",
                    Direction.W => "F",
                    _ => throw new Exception(),
                };

                if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
            }
            else if (posFrom.col + 1 == posTo.col)
            {
                move = robot.Direction switch
                {
                    Direction.N => "R",
                    Direction.E => "F",
                    Direction.S => "C",
                    Direction.W => "R",
                    _ => throw new Exception(),
                };

                if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
            }
        }
        else if (posFrom.col == posTo.col)
        {
            if (posFrom.row - 1 == posTo.row)
            {
                move = robot.Direction switch
                {
                    Direction.N => "F",
                    Direction.E => "C",
                    Direction.S => "R",
                    Direction.W => "R",
                    _ => throw new Exception(),
                };

                if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
            }
            else if (posFrom.row + 1 == posTo.row)
            {
                move = robot.Direction switch
                {
                    Direction.N => "R",
                    Direction.E => "R",
                    Direction.S => "F",
                    Direction.W => "C",
                    _ => throw new Exception(),
                };

                if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
            }
        }

        ExecuteStep(i, move);
    }

    public void ExecuteStep(int i, string move)
    {
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
                _routes[i] = _astar.AStarSearch(_robots[i]);
            }
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
