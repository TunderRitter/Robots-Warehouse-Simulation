namespace Warehouse_Simulation_Tests.Model
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Warehouse_Simulation_Model.Model;

    [TestClass]
    public class CASCellTests
    {
        private bool[,] _map;

        [TestInitialize]
        public void Setup()
        {
            _map = new bool[,]
            {
                { false, false, false, false },
                { false, true, false, false },
                { false, true, false, false },
                { false, false, false, false }
            };
        }

        [TestMethod]
        public void CanConstruct()
        {
            var instance = new CASCell(1170451990, 537964413, new CASCell(430843507, 1208397320, default(CASCell), 609786662), 1743549481);

            Assert.IsNotNull(instance);
        }


        [TestMethod]
        public void FindPath_Returns_Path_When_TargetPos_Is_Valid()
        {
            var caStar = new CAStar(_map);
            var robot = new Robot(1, (0, 0), Direction.N);
            robot.TargetPos = (2, 0);

            var path = caStar.FindPath(robot, 0);

            Assert.IsTrue(path.Count > 0);
        }

        [TestMethod]
        public void FindPath_Returns_Path_When_ReservationMap_Is_Empty()
        {
            var caStar = new CAStar(_map);
            var robot = new Robot(1, (0, 0), Direction.N);
            robot.TargetPos = (2, 0);

            var path = caStar.FindPath(robot, 0);

            Assert.IsTrue(path.Count > 0);
        }


    }
}