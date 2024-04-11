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


    private Robot[] GetRobots(Log log)
    {
        if (log.teamSize != log.start.Count)
        {
            // exception
        }
        Robot[] robots = new Robot[log.teamSize];

        return robots;
    }
}
