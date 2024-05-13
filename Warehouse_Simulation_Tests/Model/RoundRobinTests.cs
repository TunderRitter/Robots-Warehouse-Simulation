using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warehouse_Simulation_Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.Model.Tests
{
    [TestClass()]
    public class RoundRobinTests
    {
        [TestMethod()]
        public void AssignTest()
        {
            RoundRobin roundRobin = new RoundRobin();
            List<Robot> robots = new List<Robot>();
            List<Target> targets = new List<Target>();
            robots.Add(new Robot(1, (1, 1), Direction.N));
            robots.Add(new Robot(2, (2, 2), Direction.E));
            targets.Add(new Target((1, 3), 1));
            targets.Add(new Target((3, 3), 2));

            (int, int)[] result = roundRobin.Assign(robots, targets);

            Assert.AreEqual(2, result.Length);
        }
    }
}