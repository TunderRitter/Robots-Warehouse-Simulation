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
    public class ControllerTests
    {
        private bool[,] map = new bool[5, 5];
        private Robot[] robots = new Robot[2];
        [TestInitialize]
        public void Setup()
        {
            robots[0] = new Robot(1, (1, 1), Direction.N);
            robots[1] = new Robot(2, (2, 2), Direction.E);
        }

        [TestMethod]
        public void ControllerTest()
        {
            if (map == null || robots == null) Assert.Fail();

            Controller controller = new Controller(map, robots);
            Assert.AreEqual(0, controller.Step);

            controller.CalculateRoutes();
            controller.CalculateSteps();

            Assert.AreEqual(0, controller.Step);
        }
    }
}