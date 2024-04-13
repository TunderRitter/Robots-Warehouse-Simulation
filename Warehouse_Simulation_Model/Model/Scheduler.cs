using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly List<Target> _targets;
    private readonly Log _log;
    private readonly ITaskAssigner _strategy;
    private double _timeLimit;
    private readonly int _teamSize;
    private readonly int _targetsSeen;
    private int _targetCount;
    private bool _robotFreed;
    private Controller _controller;

    private const bool _passThrough = false;

    public Cell[,] Map { get; private set; }
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
        _targets = [];
        for (int i = 0; i < data.Targets.Length; i++)
        {
            Target target = new(data.Targets[i], i);
            _targets.Add(target);
            ((Floor)Map[target.Pos.row, target.Pos.col]).Target = target;
        }
        _targetCount = data.Targets.Length;

		_log = new Log();
        WriteLogStart();
        WriteLogTeamSize();

        _strategy = TaskAssignerFactory.Create(data.Strategy);

        _timeLimit = 1000; // !!!
        _targetsSeen = data.TasksSeen;
        _robotFreed = false;
        _controller = new Controller(data.Map, _robots);

        MaxSteps = 10000; // !!!
        Step = 1;
    }

    public void Schedule()
    {
        DateTime startTime, endTime;
        startTime = DateTime.Now;

        AddTargets();
        AssignTasks();
        _controller.CalculateRoutes();
        ChangeOccurred?.Invoke(this, EventArgs.Empty);

        while(Step <= MaxSteps) 
        {
            if (_robotFreed)
            {
                AddTargets();
                AssignTasks();
                _controller.CalculateRoutes();
                _robotFreed = false;
            }

            string[] steps = _controller.CalculateSteps();
            for (int i = 0; i < steps.Length; i++)
            {
                ExecuteStep(i, steps[i]);
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
            WriteLogPlannerTimes(elapsedMillisecs);
            WriteLogSumOfCost();
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

    private static void MoveRobot(Robot robot) => robot.Move();

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

        WriteLogNumTaskFinished();
    }

    public void AssignTasks()
    {
        List<Robot> free = _robots.Where(e => e.TargetPos == null).ToList();
        List<Target> assignable = _targets[..Math.Min(_targetsSeen, _targets.Count)].Where(e => e.Id == null).ToList();
        _strategy.Assign(free, assignable);
    }

    public void ExecuteStep(int robotId, string move)
    {
        Robot robot = _robots[robotId];
        switch (move)
        {
            case "F":
                ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = null;
                MoveRobot(robot);
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

    private void WriteLogStart()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            Object[] data = { _robots[i].Pos.row, _robots[i].Pos.col, _robots[i].Direction.ToString() };
            _log.start.Add(data);
        }
    }

    private void WriteLogPlannerTimes(double time)
    {
        _log.plannerTimes.Add(time);
    }

    private void WriteLogTeamSize()
    {
        _log.teamSize = _robots.Length;
    }

    private void WriteLogNumTaskFinished()
    {
        _log.numTaskFinished += 1;
    }

    private void WriteLogSumOfCost()
    {
        _log.sumOfCost += _robots.Length;
    }
    public void WriteLog()
    {

    }

    public void AddTarget(int row, int col)
    {
        if (Map[row, col] is Wall) return;

        Target target = new((row, col), _targetCount);
        _targets.Add(target);
        _targetCount++;
        ((Floor)Map[row, col]).Target = target;
    }
}
