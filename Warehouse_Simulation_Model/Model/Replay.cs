using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model
{
    public class Replay
    {
        private readonly Robot[] _robots;
        private readonly Target[] _targets;
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
