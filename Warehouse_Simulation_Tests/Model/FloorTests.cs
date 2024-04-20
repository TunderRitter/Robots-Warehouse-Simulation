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
    public class FloorTests
    {
        [TestMethod()]
        public void FloorTest()
        {
            Floor floor = new Floor();
            Robot robot = new Robot(1, (0, 0), Direction.N);
            Target target = new Target((0, 0), 1);

            Assert.AreEqual(null, floor.Robot);
            Assert.AreEqual(null, floor.Target);

            floor.Target = target;
            floor.Robot = robot;

            Assert.AreEqual(robot, floor.Robot);
            Assert.AreEqual(target, floor.Target);
        }
    }
}