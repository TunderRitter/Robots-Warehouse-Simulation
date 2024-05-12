using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class representing the scheduler of the simulation.
/// </summary>
public class Scheduler
{
    #region Fields
    /// <summary>
    /// Array representing the robots in the simulation.
    /// </summary>
    private readonly Robot[] _robots;
    /// <summary>
    /// List representing the targets in the simulation.
    /// </summary>
    private readonly List<Target> _targets;
    /// <summary>
    /// Variable representing the log of the simulation.
    /// </summary>
    private readonly Log _log;
    /// <summary>
    /// Variable representing the task assigner strategy.
    /// </summary>
    private readonly ITaskAssigner _strategy;
    /// <summary>
    /// Variable representing the controller of the simulation.
    /// </summary>
    private readonly Controller _controller;
    /// <summary>
    /// Integer representing the team size (number of robots).
    /// </summary>
    private readonly int _teamSize;
    /// <summary>
    /// Integer representing the number of targets seen by the scheduler.
    /// </summary>
    private readonly int _targetsSeen;
    /// <summary>
    /// Integer representing the number of targets in the simulation.
    /// </summary>
    private int _targetCount;
    /// <summary>
    /// Boolean representing if a robot has been freed.
    /// </summary>
    private bool _robotFreed;
    #endregion

    #region Properties
    /// <summary>
    /// Property representing the map of the simulation.
    /// </summary>
    public Cell[,] Map { get; private set; }
    /// <summary>
    /// Property representing the maximum number of steps of the simulation.
    /// </summary>
    public int MaxSteps { get; set; }
    /// <summary>
    /// Property representing the current step of the simulation.
    /// </summary>
    public int Step { get; private set; }
    /// <summary>
    /// Property representing the time limit of the simulation.
    /// </summary>
    public double TimeLimit { get; set; }
    /// <summary>
    /// Property representing if the simulation is running.
    /// </summary>
    public bool Running { get; set; }
    /// <summary>
    /// Property representing the number of robots in the simulation.
    /// </summary>
    public int RobotNum => _robots.Length;
    /// <summary>
    /// Property representing the number of targets in the simulation.
    /// </summary>
    public int TargetNum => _targets.Count;
    #endregion

    #region Events
    /// <summary>
    /// Event for when a change occurs in the simulation.
    /// </summary>
    public event EventHandler? ChangeOccurred;
    /// <summary>
    /// Event for when the simulation finishes.
    /// </summary>
    public event EventHandler? SimFinished;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Scheduler"/> class.
    /// </summary>
    /// <param name="data"></param>
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
        _targetsSeen = data.TasksSeen * data.Robots.Length;
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

    /// <summary>
    /// Method that schedules the simulation (manages time, assigns tasks, controlling robots, writes to log).
    /// </summary>
    public void Schedule()
    {
        DateTime startTime, endTime;
        startTime = DateTime.Now;

        AddTargets();
        AssignTasks();
        _controller.CalculateRoutes();
        ChangeOccurred?.Invoke(this, EventArgs.Empty);
        Running = true;

        while (Running && Step < MaxSteps)
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

            _controller.Step++;
            waitTime = Math.Min(waitTime, MaxSteps - Step);
            Step += waitTime;

            if (waitTime > 0)
            {
                for (int i = 0; i < _robots.Length; i++)
                {
                    WriteLogPlannerpaths(i, string.Join(',', Enumerable.Repeat("T", waitTime)));
                    WriteLogActualPaths(i, string.Join(',', Enumerable.Repeat("W", waitTime)));
                }
                List<object[]> timeouts = [];
                for (int i = Step - waitTime; i < Step; i++)
                {
                    timeouts.Add([-1, -1, i, "timeout"]);
                }
                WriteLogErrors(timeouts);
            }

            if (Step >= MaxSteps)
            {
                ChangeOccurred?.Invoke(this, EventArgs.Empty);
                break;
            }
            Step++;

            List<object[]> errors = CheckCollisions(steps);
            if (errors.Count != 0)
            {
                ExecuteSteps(Enumerable.Repeat("W", _robots.Length).ToArray());
                _controller.Reset();
                WriteLogErrors(errors);
            }
            else
            {
                ExecuteSteps(steps);
            }

            WriteLogMakespan();
            WriteLogPlannerTimes(elapsedMillisecs);
            WriteLogSumOfCost();

            CheckIfDone();
            ChangeOccurred?.Invoke(this, EventArgs.Empty);
        }

        WriteLog();
        SimFinished?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Method that assigns the robots to the targets.
    /// </summary>
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
    
    /// <summary>
    /// Method for checking robot collisions
    /// </summary>
    /// <param name="steps"></param>
    /// <returns>The collisions to log</returns>
    public List<object[]> CheckCollisions(string[] steps)
    {
        List<object[]> errors = [];
        Dictionary<(int, int), int> collisions = [];

        for (int i = 0; i < _robots.Length; i++)
        {
            (int, int) position;
            if (steps[i] == "F")
                position = _robots[i].NextMove();
            else
                position = _robots[i].Pos;
            if (!collisions.TryAdd(position, i))
            {
                errors.Add([
                    i,
                    collisions[position],
                    Step,
                    "collision",
                ]);
            }
        }

        return errors;
    }

    /// <summary>
    /// Method for executing the steps of the robots.
    /// </summary>
    /// <param name="steps"></param>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Method for writing to the log of the simulation.
    /// </summary>
    public void WriteLog()
    {
        WriteLogAllValid();
    }

    /// <summary>
    /// Method for adding a target to the simulation.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void AddTarget(int row, int col)
    {
        if (Map[row, col] is Wall) return;

        Target target = new((row, col), _targetCount);
        _targets.Add(target);
        _targetCount++;
        ((Floor)Map[row, col]).Target = target;

        WriteLogExtraTask(target);
    }

    /// <summary>
    /// Method for writing the log into a file.
    /// </summary>
    /// <param name="path"></param>
    public void WriteToFile(string path)
    {
        _log.Write(path);
    }

    /// <summary>
    /// Method for getting the path of a robot.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns>The route of the robor in a list of coordinates.</returns>
    public List<(int, int)> GetRobotPath(int idx)
    {
        return _controller.GetRoute(idx);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Method for adding the targets to the map of the simulation.
    /// </summary>
    private void AddTargets()
    {
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            _targets[i].Active = i < _targetsSeen;
            ((Floor)Map[_targets[i].Pos.row, _targets[i].Pos.col]).Target = _targets[i];
        }
    }

    /// <summary>
    /// Method for making the robot take a step forward.
    /// </summary>
    /// <param name="robot"></param>
    private static void MoveRobot(Robot robot) => robot.Move();

    /// <summary>
    /// Method for making the robot turn left.
    /// </summary>
    /// <param name="robot"></param>
    private static void TurnRobotLeft(Robot robot) => robot.TurnLeft();

    /// <summary>
    /// Method for making the robot turn right.
    /// </summary>
    /// <param name="robot"></param>
    private static void TurnRobotRight(Robot robot) => robot.TurnRight();

    /// <summary>
    /// Method for when a robot finishes a task (removing the target from the map and writing to the log).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Robot_Finished(object? sender, EventArgs e)
    {
        _robotFreed = true;
        if (sender is Robot robot)
        {
            Target? target = _targets.ElementAtOrDefault(_targets.FindIndex(e => e.Pos == robot.Pos));
            if (target != null)
            {
                WriteLogEvents(target.InitId, robot.Id, Step, "finished");
                _targets.Remove(target);
            }
            ((Floor)Map[robot.Pos.row, robot.Pos.col]).Target = null;
        }

        WriteLogNumTaskFinished();
    }

    /// <summary>
    /// Method for when a robot gets stuck (removing the target from the robot).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Controller_RobotStuck(object? sender, int e)
    {
        int idx = _targets.FindIndex(x => x.Pos == _robots[e].TargetPos);
        if (idx == -1) return;

        _robots[e].TargetPos = null;
        _targets[idx].Id = null;
        _robotFreed = true;
    }

    /// <summary>
    /// Method for checking if all the robots have reached their targets.
    /// </summary>
    private void CheckIfDone()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            if (_robots[i].TargetPos != null) return;
        }
        if (_targets.Count == 0)
            Running = false;
    }

    /// <summary>
    /// Method for writing the actionmodel to the log.
    /// </summary>
    /// <param name="model"></param>
    private void WriteLogActionModel(string model) => _log.actionModel = model;

    /// <summary>
    /// Method for writing the starting positions and directions of the robots to the log.
    /// </summary>
    private void WriteLogStart()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            object[] data = [_robots[i].Pos.row, _robots[i].Pos.col, _robots[i].Direction.ToString()];
            _log.start.Add(data);
        }
    }

    /// <summary>
    /// Method for writing the planner times to the log.
    /// </summary>
    /// <param name="time"></param>
    private void WriteLogPlannerTimes(double time)
    {
        _log.plannerTimes.Add((float)time / 1000f);
    }

    /// <summary>
    /// Method for writing the team size to the log.
    /// </summary>
    private void WriteLogTeamSize()
    {
        _log.teamSize = _robots.Length;
        for (int i = 0; i < _robots.Length; i++)
        {
            _log.plannerPaths.Add("");
            _log.actualPaths.Add("");
        }
    }

    /// <summary>
    /// Method for writing the number of tasks finished to the log.
    /// </summary>
    private void WriteLogNumTaskFinished()
    {
        _log.numTaskFinished += 1;
    }

    /// <summary>
    /// Method for writing the sum of cost to the log.
    /// </summary>
    private void WriteLogSumOfCost()
    {
        _log.sumOfCost += _robots.Length;
    }

    /// <summary>
    /// Method for writing the makespan to the log.
    /// </summary>
    private void WriteLogMakespan()
    {
        _log.makespan += 1;
    }

    /// <summary>
    /// Method for initializing the log events.
    /// </summary>
    private void InitLogEvents()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            _log.events.Add(new List<object[]>());
        }
    }

    /// <summary>
    /// Method for writing the events to the log.
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="robotId"></param>
    /// <param name="step"></param>
    /// <param name="_event"></param>
    private void WriteLogEvents(int taskId, int robotId, int step, string _event)
    {
        _log.events[robotId].Add([taskId, step, _event]);
    }

    /// <summary>
    /// Method for writing the actual paths of the robots to the log.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="move"></param>
    private void WriteLogActualPaths(int i, string move)
    {
        if (_log.actualPaths[i] == "")
            _log.actualPaths[i] += move;
        else
            _log.actualPaths[i] += "," + move;
    }

    /// <summary>
    /// Method for writing the planned paths of the robots to the log.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="move"></param>
    private void WriteLogPlannerpaths(int i, string move)
    {
        if (_log.plannerPaths[i] == "")
            _log.plannerPaths[i] += move;
        else
            _log.plannerPaths[i] += "," + move;
    }

    /// <summary>
    /// Method for writing the positions and ids of the targets to the log.
    /// </summary>
    private void WriteLogTasks()
    {
        for (int i = 0; i < _targets.Count; i++)
        {
            int[] trg = [_targets[i].InitId, _targets[i].Pos.row, _targets[i].Pos.col];
            _log.tasks.Add(trg);
        }
    }

    /// <summary>
    /// Method for writing the extra tasks (tasks added to the simulation) to the log.
    /// </summary>
    /// <param name="target"></param>
    private void WriteLogExtraTask(Target target)
    {
        int[] trg = [target.InitId, target.Pos.row, target.Pos.col];
        _log.tasks.Add(trg);
    }

    /// <summary>
    /// Method for writing the errors (collisions, timeout) to the log.
    /// </summary>
    /// <param name="errors"></param>
    private void WriteLogErrors(List<object[]> errors)
    {
        _log.errors.AddRange(errors);
    }

    /// <summary>
    /// Method for writing if there were errors to the log.
    /// </summary>
    private void WriteLogAllValid()
    {
        if (_log.errors.Count > 0) _log.AllValid = "No";
        else _log.AllValid = "Yes";
    }
    #endregion
}
