using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly Queue<Target> _targets;
    private readonly Log _log;
    private readonly Queue<(int, int)>[] _routes;
    private readonly ITaskAssigner _strategy;
    private readonly AStar _astar;
    private readonly double _timeLimit;
    private readonly int _teamSize;
    private bool _robotFreed;

    public Cell[,] Map { get; private set; } // Encapsulation!
    public int MaxSteps { get; set; }
    public int Step { get; private set; }

    //Fontos!!!
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
        _targets = new Queue<Target>();
        foreach ((int row, int col) targetPos in data.Targets)
        {
            Target target = new(targetPos);
            _targets.Enqueue(target);
            ((Floor)Map[targetPos.row, targetPos.col]).Target = target;
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
        _teamSize = Math.Min(data.TeamSize, data.Robots.Length);
        _robotFreed = false;

        MaxSteps = 10000; // !!!
        Step = 1;
    }

    public void Schedule()
    {
        DateTime startTime, endTime;
        startTime = DateTime.Now;

        AssignTasks();
        CalculateRoutes();

        while(Step <= MaxSteps) 
        {
            if (_robotFreed)
            {
                AssignTasks();
                _robotFreed = false;
            }

            for (int i = 0; i < _robots.Length; i++)
            {
                CalculateStep(i);
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
        }
    }

    private static void TurnRobotLeft(Robot robot) => robot.TurnLeft();

    private static void TurnRobotRight(Robot robot) => robot.TurnRight();

    private void Robot_Finished(object? sender, EventArgs e)
    {
        _robotFreed = true;
        if (sender is Robot robot)
            ((Floor)Map[robot.Pos.row, robot.Pos.col]).Target = null;
    }

    public void AssignTasks()
    {
        Robot[] freeAll = _robots.Where(e => e.TargetPos == null).ToArray();
        int len = Math.Min(_targets.Count, Math.Min(_teamSize, freeAll.Length));
        Robot[] free = new Robot[len];
        Array.Copy(freeAll, free, len);

        Target[] targets = new Target[len];
        for (int i = 0; i < len; i++)
        {
            targets[i] = _targets.Dequeue();
        }

        _strategy.Assign(free, targets);
		for (int i = 0; i < len; i++)
		{
			((Floor)Map[targets[i].Pos.row, targets[i].Pos.col]).Target = targets[i];
		}
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
        if (Map[row, col] is Wall || (Map[row, col] is Floor floor && floor.Target != null)) return;

        Target target = new((row, col));
        _targets.Enqueue(target);
        ((Floor)Map[row, col]).Target = target;
    }
}
