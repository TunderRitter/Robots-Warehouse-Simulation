using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse_Simulation_Model.persistence;

namespace Warehouse_Simulation_Model
{
    public class Replay
    {
        private readonly Robot[] _robots;
        private readonly Target[] _targets; // List?
        private double _speed;
        private bool _paused;
        private readonly Cell[][,] _map;
        public Cell[][,] Map => _map;


        public Replay()
        {

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
    }
}
