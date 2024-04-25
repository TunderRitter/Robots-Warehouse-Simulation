using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warehouse_Simulation_Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse_Simulation_Model.Persistence;

namespace Warehouse_Simulation_Model.Model.Tests
{
    [TestClass()]
    public class SchedulerTests
    {
        private bool[,] map;
        private (int, int)[] robots;
        private (int, int)[] targets;

        [TestInitialize]

        public void Setup()
        {
            map = new bool[5, 5]
            {
                {true, true, true, true, true},
                {true, false, false, false, true},
                {true, false, true, false, true},
                {true, false, false, false, true},
                {true, true, true, true, true}
            };
            robots = new (int, int)[2]
            {
                (1, 1),
                (3, 1)
            };
            targets = new (int, int)[2]
            {
                (1, 3),
                (3, 3)
            };
        }

        [TestMethod()]
        public void SchedulerTest()
        {          
            SchedulerData data = new SchedulerData
            {
                Map = map,
                Robots = robots,
                Targets = targets,
                TeamSize = 2,
                TasksSeen = 2,
                Strategy = "roundrobin",
            };

            Scheduler scheduler = new Scheduler(data);

            Assert.AreEqual(2, scheduler.RobotNum);
            Assert.AreEqual(2, scheduler.TargetNum);
            Assert.AreEqual(false, scheduler.Running);
            Assert.AreEqual(1000, scheduler.TimeLimit);
            Assert.AreEqual(0, scheduler.Step);
            Assert.AreEqual(10000, scheduler.MaxSteps);
        }

        [TestMethod()]
        public void ScheduleTest()
        {
            SchedulerData data = new SchedulerData
            {
                Map = map,
                Robots = robots,
                Targets = targets,
                TeamSize = 2,
                TasksSeen = 2,
                Strategy = "roundrobin",
            };

            Scheduler scheduler = new Scheduler(data);
            scheduler.MaxSteps = 1;

            scheduler.Schedule();

            Assert.AreEqual(2, scheduler.Step);
            Assert.AreEqual(true, scheduler.Running);
        }
    }
}