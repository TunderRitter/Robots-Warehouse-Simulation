using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Warehouse_Simulation_Model.persistence;

namespace Warehouse_Simulation_Model;


public class Scheduler
{
    private readonly Robot[] _robots;
    private readonly Queue<Target> _targets;
    private readonly double _timeLimit;
    private readonly Log _log;
    private readonly Queue<(int, int)>[] _routes;
    private readonly ITaskAssigner _method;

    private Cell[,] _map;
    public Cell[,] Map => _map;
    public int Steps { get; private set; }


    public Scheduler(int mapWidth, int mapHeight, Robot[] robots, double timeLimit, Log log, ITaskAssigner method)
    {

    }


    private List<(int, int)> AStar()
    {
        return null;
    }

    private void TurnRobot(string str, Robot robot)
    {

    }

    private void RobotFinished(Robot robot)
    {

    }

    public void RemoveTarget(Target target)
    {

    }

    public void AssignTask(Robot robot)
    {

    }

    public void CalculateStep()
    {

    }

    public void ExecuteStep()
    {

    }

    public void CalculateRoutes(Robot[] robots)
    {

    }

    public void WriteLog()
    {

    }

    public void AddTarget(int x, int y)
    {

    }

}
