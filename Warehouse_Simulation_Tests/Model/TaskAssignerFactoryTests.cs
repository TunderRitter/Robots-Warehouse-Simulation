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
    public class TaskAssignerFactoryTests
    {
        [TestMethod()]
        public void CreateTest()
        {
            Assert.ThrowsException<InvalidDataException>(() => TaskAssignerFactory.Create("not supported"));
        }
    }
}