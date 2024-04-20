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
    public class AStarCellTests
    {
        [TestMethod()]
        public void AStarCellTest()
        {
            int i = 2;
            int j = 3;
            AStarCell? parent = new AStarCell(1, 1, null);

            AStarCell cell = new AStarCell(i, j, parent);

            Assert.AreEqual(i, cell.I);
            Assert.AreEqual(j, cell.J);
            Assert.AreEqual(parent, cell.Parent);
            Assert.AreEqual(0, cell.f);
            Assert.AreEqual(0, cell.g);
            Assert.AreEqual(0, cell.h);
        }
    }
}