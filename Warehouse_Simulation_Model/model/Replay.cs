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
        private Robot[] _robots;
        private Target[] _targets;
        private Cell[][,] _map;
        public Cell[][,] Map
        {
            get { return _map; }
            set { _map = value; }
        }


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

        public void ChangeSpeed()
        {
             
        }

        public void StepFwd()
        {

        }

        public void StepBack()
        {

        }

        public void SkipTo()
        {

        }

        public void GenerateMaps()
        {

        }
    }
}
