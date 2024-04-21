using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly List<Target> _targets;
    private readonly Log _log;
    private readonly ITaskAssigner _strategy;
    private readonly Controller _controller;
    private readonly int _teamSize;
    private readonly int _targetsSeen;
    private int _targetCount;
    private bool _robotFreed;

    public Cell[,] Map { get; private set; }
    public int MaxSteps { get; set; }
    public int Step { get; private set; }
    public double TimeLimit { get; set; }
    public bool Running { get; set; }

    public int RobotNum => _robots.Length;
    public int TargetNum => _targets.Count;

    public event EventHandler? ChangeOccurred;
    public event EventHandler? SimFinished;


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

        _strategy = TaskAssignerFactory.Create(data.Strategy);
        _targetsSeen = data.TasksSeen;
        _robotFreed = false;
        _controller = new Controller(data.Map, _robots);
        _controller.RobotStuck += Controller_RobotStuck;

        TimeLimit = 1000;
        MaxSteps = 10000;
        Step = 0;
        Running = false;

        _log = new Log();
        WriteLogActionModel("MAPF_T");
        WriteLogStart();
        WriteLogTeamSize();
        WriteLogTasks();
        InitLogEvents();
    }


    public void Schedule()
    {
        DateTime startTime, endTime;
        startTime = DateTime.Now;

        AddTargets();
        AssignTasks();
        _controller.CalculateRoutes();
        ChangeOccurred?.Invoke(this, EventArgs.Empty);
        Running = true;

        while(Running && Step <= MaxSteps) 
        {
            if (_robotFreed)
            {
                AddTargets();
                AssignTasks();
                _robotFreed = false;
            }

            _controller.CalculateRoutes();
            string[] steps = _controller.CalculateSteps();

            endTime = DateTime.Now;
            double elapsedMillisecs = (endTime - startTime).TotalMilliseconds;
            int waitTime = (int)(elapsedMillisecs / TimeLimit);
            Thread.Sleep((int)(TimeLimit * (waitTime + 1) - elapsedMillisecs));
            startTime = DateTime.Now;

            // DEBUG
            waitTime = 0;

            Step += waitTime + 1;
            _controller.Step++;
            if (Step > MaxSteps) break;
            
            if (waitTime > 0)
            {
                for (int i = 0; i < _robots.Length; i++)
                {
                    WriteLogPlannerpaths(i, string.Join(',', Enumerable.Repeat("T", waitTime)));
                    WriteLogActualPaths(i, string.Join(',', Enumerable.Repeat("W", waitTime)));
                }
            }
            ExecuteSteps(steps);

            WriteLogMakespan();
            WriteLogPlannerTimes(elapsedMillisecs);
            WriteLogSumOfCost();

            CheckIfDone();
            ChangeOccurred?.Invoke(this, EventArgs.Empty);
        }

        WriteLog();
        SimFinished?.Invoke(this, EventArgs.Empty);
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
            Target? target = _targets.ElementAtOrDefault(_targets.FindIndex(e => e.Pos == robot.Pos));
            if(target != null)
            {
                WriteLogEvents(target.InitId, robot.Id, Step, "finished");
                _targets.Remove(target);
            }
            ((Floor)Map[robot.Pos.row, robot.Pos.col]).Target = null;
        }

        WriteLogNumTaskFinished();
    }

    private void Controller_RobotStuck(object? sender, int e)
    {
        int idx = _targets.FindIndex(x => x.Pos == _robots[e].TargetPos);
        if (idx == -1) return;

        _robots[e].TargetPos = null;
        _targets[idx].Id = null;
        _robotFreed = true;
    }

    public void AssignTasks()
    {
        List<Robot> free = _robots.Where(e => e.TargetPos == null).ToList();
        List<Target> assignable = _targets[..Math.Min(_targetsSeen, _targets.Count)].Where(
            e => e.Id == null && !_targets.Where(x => x.Id != null).Select(x => x.Pos).Contains(e.Pos)
        ).ToList();

        (int, int)[] assignments = _strategy.Assign(free, assignable);
        foreach ((int robotId, int targetId) in assignments)
        {
            WriteLogEvents(targetId, robotId, Step, "assigned");
        }
    }

    public void ExecuteSteps(string[] steps)
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            Robot robot = _robots[i];
            switch (steps[i])
            {
                case "F":
                    ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = null;
                    MoveRobot(robot);
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

            //write log
            WriteLogPlannerpaths(i, steps[i]);
            WriteLogActualPaths(i, steps[i]);
        }
        for (int i = 0; i < _robots.Length; i++)
        {
            Robot robot = _robots[i];
            ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = robot;
            _robots[i].CheckPos();
        }
        
    }

    private void CheckIfDone()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            if (_robots[i].TargetPos != null) return;
        }
        if (_targets.Count == 0)
            Running = false;
    }


    private void WriteLogActionModel(string model) => _log.actionModel = model;

    private void WriteLogStart()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            object[] data = [_robots[i].Pos.row, _robots[i].Pos.col, _robots[i].Direction.ToString()];
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
        for (int i = 0; i < _robots.Length; i++)
        {
            _log.plannerPaths.Add("");
            _log.actualPaths.Add("");
        }
    }

    private void WriteLogNumTaskFinished()
    {
        _log.numTaskFinished += 1;
    }

    private void WriteLogSumOfCost()
    {
        _log.sumOfCost += _robots.Length;
    }

    private void WriteLogMakespan()
    {
        _log.makespan += 1;
    }

    private void InitLogEvents()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            _log.events.Add(new List<object[]>());
        }
    }

    private void WriteLogEvents(int taskId, int robotId,  int step, string _event)
    {
        _log.events[robotId].Add(new object[] { taskId, step, _event });
    }

    private void WriteLogActualPaths(int i, string move)
    {
        if (_log.actualPaths[i] == "")
            _log.actualPaths[i] += move;
        else
            _log.actualPaths[i] += "," + move;
    }

    private void WriteLogPlannerpaths(int i, string move)
    {
        if (_log.plannerPaths[i] == "")
            _log.plannerPaths[i] += move;
        else
            _log.plannerPaths[i] += "," + move;
    }

    private void WriteLogTasks()
    {
        for (int i = 0; i < _targets.Count; i++)
        {
            int[] trg = { _targets[i].InitId, _targets[i].Pos.row, _targets[i].Pos.col };
            _log.tasks.Add(trg);
        }
    }

    private void WriteLogErrors()
    {

    }

    private void WriteLogAllValid()
    {
        if (_log.errors.Count > 0) _log.AllValid = "No";
        else _log.AllValid = "Yes";
    }

    public void WriteLog()
    {
        WriteLogAllValid();
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
