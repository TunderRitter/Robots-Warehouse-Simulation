using Warehouse_Simulation_Model.Persistence;
using System.Timers;

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
    private readonly int _steps;
    private readonly int _teamSize;
    private bool _robotFreed;

    public Cell[,] Map { get; private set; } // Encapsulation!
    public int Steps { get; private set; }

    //Fontos!!!
    //Minden függvényben hívjátok meg pls, mert ez értesíti a viewmodelt MINDEN változásról a schedulerben!!!!
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
        }
        _targets = new Queue<Target>();
        foreach ((int row, int col) target in data.Targets)
        {
            _targets.Enqueue(new Target(target));
        }

        _log = new Log();
        _routes = new Queue<(int, int)>[data.Robots.Length];
        _strategy = TaskAssignerFactory.Create(data.Strategy);
        _astar = new AStar(data.Map);

        _timeLimit = 10; // !!!
        _steps = 10; // !!!
        _teamSize = Math.Min(data.TeamSize, data.Robots.Length);
        _robotFreed = false;


        //innen majd szedjétek ki a kikommentelést!!!

        //AssignTasks();

        //Schedule();
    }

    private void Schedule()
    {
        //System.Timers.Timer timer = new System.Timers.Timer();
        if (_robotFreed)
        {
            AssignTasks();
            _robotFreed = false;
        }

        CalculateRoutes();

        while(_targets.Count > 0 && Steps >= _steps) 
        {
            for (int i = 0; i < _robots.Length; i++)
            {
                CalculateStep(_robots[i], i);
            }
            //várjon az időlimitig, vagy ha túllépte akkor várjon megint annyit
        }

        //System.Threading.Thread.Sleep(1000);
    }

    private static void TurnRobotLeft(Robot robot) => robot.TurnLeft();

    private static void TurnRobotRight(Robot robot) => robot.TurnRight();

    private void Robot_Finished(object? sender, int e) => _robotFreed = true;

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
