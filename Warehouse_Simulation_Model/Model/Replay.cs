using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Replay
{
    private readonly Log _log;
    private readonly Robot[] _robots;
    private readonly Target[] _targets;
    private readonly List<string>[] _steps;
    private double _speed;
    private bool _paused;
    public Cell[,] InitMap { get; init; }
    public Cell[][,] Maps { get; init; }
    public int Step { get; set; }


    public Replay(string logPath, string mapPath)
    {
        _log = Log.Read(logPath);
        _robots = GetRobots(_log);
        bool[,] mapBool = ConfigReader.ReadMap(mapPath);
        _targets = GetTargets(_log);
        _steps = GetSteps(_log);
        InitMap = GetMap(mapBool, _robots, _targets);
        _speed = 1.0;
        _paused = true;
        Maps = new Cell[_log.sumOfCost / _log.plannerPaths.Count][,];
    }


    public void Play()
    {

    }

    public void Pause()
    {

    }

    public void ChangeSpeed(double speed)
    {
         
    }

    public void StepFwd()
    {

    }

    public void StepBack()
    {

    }

    public void SkipTo(int step)
    {

    }

    public void GenerateMaps()
    {
        Maps[0] = InitMap;
        for (int i = 1; i < Maps.Length; i++)
        {
            for (int j = 0; j < _robots.Length; j++)
            {
                Robot robot = _robots[j];
                switch (_steps[j][i - 1])
                {
                    case "F":
                        ((Floor)Maps[i][robot.Pos.row, robot.Pos.col]).Robot = null;
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
                    ((Floor)Maps[i][robot.Pos.row, robot.Pos.col]).Robot = robot;
            }
        }
    }

    private static List<string>[] GetSteps(Log log)
    {
        List<string>[] steps = new List<string>[log.start.Count];
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i] = [];
            string[] moves = log.actualPaths[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
            steps[i].AddRange(moves);
        }

        return steps;
    }


    private static Robot[] GetRobots(Log log)
    {
        Robot[] robots = new Robot[log.teamSize];
        try
        {
            if (log.teamSize != log.start.Count) throw new InvalidDataException("Invalid number of robots");

            for (int i = 0; i < robots.Length; i++)
            {
                if (log.start[i].Length == 3
                    && log.start[i][0] is int row
                    && log.start[i][1] is int col
                    && log.start[i][2] is string directionStr)
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

    private static Target[] GetTargets(Log log)
    {
        Target[] targets = new Target[log.tasks.Count];
        try
        {
            for (int i = 0; i < log.tasks.Count; i++)
            {
                if (log.tasks[i].Length != 3) throw new InvalidDataException("Invalid tasks");
                targets[i] = new Target((log.tasks[i][1], log.tasks[i][2]), log.tasks[i][0]);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return targets;
    }

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
        for (int i = 0; i < robots.Length; i++)
        {
            if (map[robots[i].Pos.row, robots[i].Pos.col] is Floor floor)
                floor.Robot = robots[i];
        }
        for (int i = targets.Length - 1; i >= 0; i--)
        {
            if (map[targets[i].Pos.row, targets[i].Pos.col] is Floor floor)
                floor.Target = targets[i];
        }

        return map;
    }
}
