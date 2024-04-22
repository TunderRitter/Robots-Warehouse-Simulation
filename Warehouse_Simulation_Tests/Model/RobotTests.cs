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
    public class RobotTests
    {
        [TestMethod()]
        public void RobotTest()
        {
            Robot robot = new(0, (0, 0), Direction.N);

            Assert.AreEqual(0, robot.Id);
            Assert.AreEqual((0, 0), robot.Pos);
            Assert.AreEqual(null, robot.TargetPos);
            Assert.AreEqual(Direction.N, robot.Direction);
        }

        [TestMethod()]
        public void MoveTest()
        {
            Robot robot = new(0, (1, 1), Direction.N);
            robot.Move();
            Assert.AreEqual((0, 1), robot.Pos);

            robot = new Robot(0, (1, 1), Direction.E);
            robot.Move();
            Assert.AreEqual((1, 2), robot.Pos);

            robot = new Robot(0, (1, 1), Direction.S);
            robot.Move();
            Assert.AreEqual((2, 1), robot.Pos);

            robot = new Robot(0, (1, 1), Direction.W);
            robot.Move();
            Assert.AreEqual((1, 0), robot.Pos);
        }

        [TestMethod()]
        public void TurnRightTest()
        {
            Robot robot = new(0, (0, 0), Direction.N);
            robot.TurnRight();
            Assert.AreEqual(Direction.E, robot.Direction);

            robot = new Robot(0, (0, 0), Direction.E);
            robot.TurnRight();
            Assert.AreEqual(Direction.S, robot.Direction);

            robot = new Robot(0, (0, 0), Direction.S);
            robot.TurnRight();
            Assert.AreEqual(Direction.W, robot.Direction);

            robot = new Robot(0, (0, 0), Direction.W);
            robot.TurnRight();
            Assert.AreEqual(Direction.N, robot.Direction);
        }

        [TestMethod()]
        public void TurnLeftTest()
        {
            Robot robot = new(0, (0, 0), Direction.N);
            robot.TurnLeft();
            Assert.AreEqual(Direction.W, robot.Direction);

            robot = new Robot(0, (0, 0), Direction.E);
            robot.TurnLeft();
            Assert.AreEqual(Direction.N, robot.Direction);

            robot = new Robot(0, (0, 0), Direction.S);
            robot.TurnLeft();
            Assert.AreEqual(Direction.E, robot.Direction);

            robot = new Robot(0, (0, 0), Direction.W);
            robot.TurnLeft();
            Assert.AreEqual(Direction.S, robot.Direction);
        }
    }
}