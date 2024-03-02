using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model
{
    public abstract class Cell
    {

    }

    public class Wall : Cell
    {
        public Wall()
        {

        }
    }

    public class  Floor : Cell
    {
        private Robot? _robot;
        public Robot? Robot
        {
            get { return _robot; }
            set { _robot = value; }
        }

        private Target? _target;
        public Target? Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Floor(Robot robot, Target target)
        {
            _robot = robot;
            _target = target;
        }
    }
}
