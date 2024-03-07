using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Warehouse_Simulation_Model.Model;

namespace Warehouse_Simulation_Model.Persistence
{
    public class SchedulerData
    {
        public (int, int)[]? _robots { get; private set; }
        public (int, int)[]? _targets { get; private set; }
        public int tasksSeen { get; private set; }
        public bool[,]? _map { get; private set; }

        public SchedulerData((int, int)[] robots, (int, int)[] targets, int tasksSeen, bool[,] map)
        {
            if (map == null) throw new ArgumentNullException("Map is null");
            if (robots == null) throw new ArgumentNullException("Robots are null");
            if (targets == null) throw new ArgumentNullException("Targets are null");
            if (tasksSeen <= 0) throw new ArgumentOutOfRangeException("Number of seen tasks must be above zero");

            this.tasksSeen = tasksSeen;
            if(checkMap(map)) _map = map;
            if (checkRobots(robots, map)) _robots = robots;
            if (checkTargets(targets, map)) _targets = targets;
        }

        private bool checkMap(bool[,] map)
        {
            if (map.Length == 0 || map.GetLength(0) == 0) throw new ArgumentOutOfRangeException("Map must be at least 1x1");

            bool allwall = true;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map.GetLength(i); j++)
                {
                    if (!map[i, j]) allwall = false;
                }
            }

            if (allwall) throw new ArgumentException("Map can't be just walls");

            return true;
        }

        private bool checkRobots((int x, int y)[] robots, bool[,] map)
        {
            for (int i = 0; i < robots.Length; i++)
            {
                if (robots[i].x < 0 || robots[i].y < 0 || robots[i].y > map.Length || robots[i].x > map.GetLength(0)) throw new ArgumentOutOfRangeException("Robots' coordinates must be within 0 and the size of the map");
                if (map[robots[i].x, robots[i].y]) throw new ArgumentException("Robot's coordinates can't be within a wall");
            }
            return true;
        }

        private bool checkTargets((int x, int y)[] targets, bool[,] map)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].x < 0 || targets[i].y < 0 || targets[i].y > map.Length || targets[i].x > map.GetLength(0)) throw new ArgumentOutOfRangeException("Targets' coordinates must be within 0 and the size of the map");
                if (map[targets[i].x, targets[i].y]) throw new ArgumentException("Target's coordinates can't be within a wall");
            }
            return true;
        }
    }
}
