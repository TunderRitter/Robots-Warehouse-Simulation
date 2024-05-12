using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class responsible for replaying the simulation.
/// </summary>
public class Replay
{
    #region Fields
    /// <summary>
    /// Variable representing the log of the replay.
    /// </summary>
    private readonly Log _log;
    /// <summary>
    /// Array representing the robots in the replay.
    /// </summary>
    private readonly Robot[] _robots;
    /// <summary>
    /// Array representing the targets in the replay.
    /// </summary>
    private readonly Target[] _targets;
    /// <summary>
    /// Array representing the steps of the replay.
    /// </summary>
    private readonly List<string>[] _steps;
    #endregion

    #region Properties
    /// <summary>
    /// Property representing the speed of the replay.
    /// </summary>
    public double Speed { get; private set; }
    /// <summary>
    /// Property representing if the replay is paused.
    /// </summary>
    public bool Paused { get; private set; }
    /// <summary>
    /// Matrix representing the map currently shown in the replay.
    /// </summary>
    public Cell[,] Map { get; init; }
    /// <summary>
    /// Matrix representing the initial map of the replay.
    /// </summary>
    public Cell[,] InitMap { get; init; }
    /// <summary>
    /// Three-dimensional array representing the maps of the replay.
    /// </summary>
    public int[][,] Maps { get; init; }
    /// <summary>
    /// Property representing the current step of the replay.
    /// </summary>
    public int Step { get; private set; }
    /// <summary>
    /// Property representing the maximum step of the replay.
    /// </summary>
    public int MaxStep { get; init; }
    /// <summary>
    /// Property representing the number of robots in the replay.
    /// </summary>
    public int RobotNum => _robots.Length;
    /// <summary>
    /// property representing the number of targets in the replay.
    /// </summary>
    public int TargetNum {
        get
        {
            int num = 0;
            foreach (int cell in Maps[Step])
            {
                if (cell < 0)
                    num++;
            }
            return num;
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Event that is triggered when a change occurs in the replay.
    /// </summary>
    public event EventHandler<int>? ChangeOccurred;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Replay"/> class.
    /// </summary>
    /// <param name="logPath"></param>
    /// <param name="mapPath"></param>
    public Replay(string logPath, string mapPath)
    {
        _log = Log.Read(logPath);
        _robots = GetRobots(_log);
        Robot[] initRobots = GetRobots(_log);
        bool[,] mapBool = ConfigReader.ReadMap(mapPath);
        _targets = GetTargets(_log);
        Target[] initTargets = GetTargets(_log);
        _steps = GetSteps(_log);
        Map = GetMap(mapBool, _robots, _targets);
        InitMap = GetMap(mapBool, initRobots, initTargets);
        Step = 0;
        Speed = 1.0;
        Paused = true;
        MaxStep = _log.SumOfCost / _log.PlannerPaths.Count;
        Maps = new int[MaxStep + 1][,];
        GenerateMaps();
    }

    /// <summary>
    /// Method that starts the replay.
    /// </summary>
    public void Start()
    {
        Thread.Sleep(1000);
        Play();
    }

    /// <summary>
    /// Method that plays the replay.
    /// </summary>
    public void Play()
    {
        Paused = false;
        Task.Run(Playing);
    }

    /// <summary>
    /// Method that pauses the replay.
    /// </summary>
    public void Pause()
    {
        Paused = true;
    }

    /// <summary>
    /// Method that doubles the speed of the replay.
    /// </summary>
    public void FasterSpeed()
    {
        if (Speed >= 8) return;
        Speed *= 2;
    }

    /// <summary>
    /// Method that halves the speed of the replay.
    /// </summary>
    public void SlowerSpeed()
    {
        if (Speed <= 0.125) return;
        Speed *= 0.5;
    }

    /// <summary>
    /// Method that steps forward in the replay.
    /// </summary>
    public void StepFwd()
    {
        Step = Math.Min(Step + 1, MaxStep);
        OnChangeOccured();
    }

    /// <summary>
    /// Method that steps back in the replay.
    /// </summary>
    public void StepBack()
    {
        Step = Math.Max(Step - 1, 0);
        OnChangeOccured();
    }

    /// <summary>
    /// Method that skips to a specific step in the replay.
    /// </summary>
    /// <param name="step"></param>
    public void SkipTo(int step)
    {
        Step = Math.Clamp(step, 0, MaxStep);
        OnChangeOccured();
    }

    /// <summary>
    /// Method that plays the replay.
    /// </summary>
    public void Playing()
    {
        while (!Paused)
        {
            StepFwd();
            Thread.Sleep((int)(1000 / Speed));
        }
    }

    /// <summary>
    /// Method that generates the maps of the replay.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void GenerateMaps()
    {
        Maps[0] = CompressMap(Map);
        for (int i = 1; i < Maps.Length; i++)
        {
            for (int j = 0; j < _robots.Length; j++)
            {
                foreach (object[] targetEvent in _log.Events[j])
                {
                    if ((int)targetEvent[1] == i - 1)
                    {
                        Target target = _targets.First(e => e.InitId == (int)targetEvent[0]);
                        if ((string)targetEvent[2] == "assigned")
                        {
                            target.Id = j;
                        }
                        else if ((string)targetEvent[2] == "finished")
                        {
                            target.Active = false;
                            if (Map[target.Pos.row, target.Pos.col] is Floor floor)
                                floor.Target = null;
                        }
                    }
                }
            }
            for (int j = _targets.Length - 1; j >= 0; j--)
            {
                if (_targets[j].Active && Map[_targets[j].Pos.row, _targets[j].Pos.col] is Floor floor)
                    floor.Target = _targets[j];
            }

            for (int j = 0; j < _robots.Length; j++)
            {
                Robot robot = _robots[j];
                switch (_steps[j][i - 1])
                {
                    case "F":
                        ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = null;
                        robot.Move();
                        break;
                    case "C":
                        robot.TurnLeft();
                        break;
                    case "R":
                        robot.TurnRight();
                        break;
                    case "W":
                        break;
                    default:
                        throw new InvalidOperationException("Invalid move");
                }
            }
            for (int j = 0; j < _robots.Length; j++)
            {
                Robot robot = _robots[j];
                if (_steps[j][i - 1] == "F")
                    ((Floor)Map[robot.Pos.row, robot.Pos.col]).Robot = robot;
            }

            Maps[i] = CompressMap(Map);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Method that compresses the map into numbers.
    /// </summary>
    /// <param name="cellMap"></param>
    /// <returns>An <see langword="int"/>[,] representing the map.</returns>
    /// <exception cref="Exception"></exception>
    private static int[,] CompressMap(Cell[,] cellMap)
    {
        int rows = cellMap.GetLength(0);
        int cols = cellMap.GetLength(1);
        
        int[,] map = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (cellMap[i, j] is Wall)
                {
                    map[i, j] = 0;
                }
                else
                {
                    Floor cellFloor = (Floor)cellMap[i, j];
                    if (cellFloor.Robot != null)
                    {
                        int direction = cellFloor.Robot.Direction switch
                        {
                            Direction.N => 0,
                            Direction.E => 1,
                            Direction.S => 2,
                            Direction.W => 3,
                            _ => throw new Exception(),
                        };
                        map[i, j] = (cellFloor.Robot.Id + 2) * 10 + direction;
                    }
                    else if (cellFloor.Target != null && cellFloor.Target.Id != null)
                        map[i, j] = -(cellFloor.Target!.Id!.Value + 2);
                    else if (cellFloor.Target != null)
                        map[i, j] = -1;
                    else
                        map[i, j] = 1;
                }
            }
        }

        return map;
    }

    /// <summary>
    /// Method that sets the steps of the robots of the replay.
    /// </summary>
    /// <param name="log"></param>
    /// <returns>The steps as a list of strings.</returns>
    private static List<string>[] GetSteps(Log log)
    {
        List<string>[] steps = new List<string>[log.Start.Count];
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i] = [];
            string[] moves = log.ActualPaths[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
            steps[i].AddRange(moves);
        }

        return steps;
    }

    /// <summary>
    /// Method that sets the robots of the replay.
    /// </summary>
    /// <param name="log"></param>
    /// <returns>An array of the robots.</returns>
    /// <exception cref="InvalidDataException"></exception>
    private static Robot[] GetRobots(Log log)
    {
        Robot[] robots = new Robot[log.TeamSize];
        try
        {
            if (log.TeamSize != log.Start.Count) throw new InvalidDataException("Invalid number of robots");

            for (int i = 0; i < robots.Length; i++)
            {
                if (log.Start[i].Length == 3
                    && log.Start[i][0] is int row
                    && log.Start[i][1] is int col
                    && log.Start[i][2] is string directionStr)
                {
                    if (!Enum.TryParse(directionStr, out Direction direction)) throw new InvalidDataException("Invalid direction");
                    robots[i] = new Robot(i, (row, col), direction);
                }
                else throw new InvalidDataException("Invalid robot start states");
            }
        }
        catch (Exception)
        {
            throw;
        }

        return robots;
    }

    /// <summary>
    /// Method that sets the targets of the replay.
    /// </summary>
    /// <param name="log"></param>
    /// <returns>An array of the targets.</returns>
    /// <exception cref="InvalidDataException"></exception>
    private static Target[] GetTargets(Log log)
    {
        Target[] targets = new Target[log.Tasks.Count];
        try
        {
            for (int i = 0; i < log.Tasks.Count; i++)
            {
                if (log.Tasks[i].Length != 3) throw new InvalidDataException("Invalid tasks");
                targets[i] = new Target((log.Tasks[i][1], log.Tasks[i][2]), log.Tasks[i][0])
                {
                    Active = true
                };
            }
        }
        catch (Exception)
        {
            throw;
        }

        return targets;
    }

    /// <summary>
    /// Method that sets the map of the replay.
    /// </summary>
    /// <param name="mapBool"></param>
    /// <param name="robots"></param>
    /// <param name="targets"></param>
    /// <returns>The map.</returns>
    private static Cell[,] GetMap(bool[,] mapBool, Robot[] robots, Target[] targets)
    {
        int height = mapBool.GetLength(0);
        int width = mapBool.GetLength(1);
        Cell[,] map = new Cell[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (mapBool[i, j])
                    map[i, j] = new Wall();
                else
                    map[i, j] = new Floor();
            }
        }
        for (int i = targets.Length - 1; i >= 0; i--)
        {
            if (targets[i].Active && map[targets[i].Pos.row, targets[i].Pos.col] is Floor floor)
                floor.Target = targets[i];
        }
        for (int i = 0; i < robots.Length; i++)
        {
            if (map[robots[i].Pos.row, robots[i].Pos.col] is Floor floor)
                floor.Robot = robots[i];
        }

        return map;
    }

    /// <summary>
    /// Method that triggers the change event.
    /// </summary>
    private void OnChangeOccured() => ChangeOccurred?.Invoke(this, Step);
    #endregion
}
