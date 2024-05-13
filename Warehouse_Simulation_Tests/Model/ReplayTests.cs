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
    public class ReplayTests
    {
        [TestMethod()]
        public void ReplayTest()
        {
            Replay replay;
            Assert.ThrowsException<FormatException>(() => replay = new Replay("asd", "asd"));
        }
    }
}