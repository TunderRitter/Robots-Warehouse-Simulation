using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Warehouse_Simulation_Model.persistence;

namespace Warehouse_Simulation_Model
{
    public class Scheduler
    {
        private Robot[] _robot;
        private Queue<Target> _targets;
        private double _timeLimit;
        private Log _log;
        private Queue<(int, int)>[] _routes;
        private ITaskAssigner _method;

        private Cell[,] _map;
        public Cell[,] Map
        {
            get { return _map; }
            set { _map = value; }
        }
        private int _steps;
        public int Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }


        public Scheduler(Scheduler scheduler)
        {

        }

        private List<(int, int)> AStar()
        {
            return null;
        }

        private void TurnRobot(String str, Robot robot)
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
}
