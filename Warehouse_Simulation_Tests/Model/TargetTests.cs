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
    public class TargetTests
    {
        [TestMethod()]
        public void TargetTest()
        {
            Target target = new Target((1, 3), 1);

            Assert.AreEqual(1, target.Pos.row);
            Assert.AreEqual(3, target.Pos.col);
            Assert.AreEqual(false, target.Active);
            Assert.AreEqual(1, target.InitId);
            Assert.AreEqual(null, target.Id);
        }
    }
}