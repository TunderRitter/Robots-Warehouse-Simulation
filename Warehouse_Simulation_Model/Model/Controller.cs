using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.Model
{
    public class Controller
    {
        private readonly Robot[] _robots;
        private readonly Queue<(int, int)>[] _routes;
        private readonly AStar _astar;
        private const bool _passThrough = false;
        public Cell[,] Map { get; private set; }
    }
}
