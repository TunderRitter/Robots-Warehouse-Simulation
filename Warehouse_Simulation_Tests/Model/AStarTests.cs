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
    public class AStarTests
    {
        private bool[,] CreateMap()
        {
            return new bool[,]
            {
            {true, true, true, true, true},
            {true, false, false, false, true},
            {true, true, true, true, true}
            };
        }

        [TestMethod()]
        public void Lowest_f_costTest()
        {
            var list = new List<AStarCell>
            {
                new AStarCell(0, 0, null) { f = 10 },
                new AStarCell(0, 0, null) { f = 5 },
                new AStarCell(0, 0, null) { f = 15 }
            };

            int result = AStar.Lowest_f_cost(list);

            Assert.AreEqual(1, result);
        }

        [TestMethod()]
        public void AStarSearchTest()
        {

            //valaki pls
        }
    }
}