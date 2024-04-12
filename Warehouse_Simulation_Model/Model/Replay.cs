using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model;


public class Replay
{
    private readonly Robot[] _robots;
    private readonly Target[] _targets;
    private double _speed;
    private bool _paused;
    public Cell[][,] Maps { get; private set; }


    public Replay(string logPath)
    {
        Log log = Log.Read(logPath);
        _robots = GetRobots(log);
        _targets = GetTargets(log, 0);
    }

    public void Read(Log log)
    {

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
                    &&log.start[i][0] is int row
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

    private static Target[] GetTargets(Log log, int mapWidth)
    {
        Target[] targets = new Target[log.tasks.Count];
        try
        {
            for (int i = 0; i < log.tasks.Count; i++)
            {
                if (log.tasks[i].Length != 3) throw new InvalidDataException("Invalid tasks");
                targets[i] = new Target(ConvertCoordinates(log.tasks[i][2], mapWidth), log.tasks[i][0]);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return targets;
    }

    private static (int, int) ConvertCoordinates(int coor, int width) => (coor / width, coor % width);
}
