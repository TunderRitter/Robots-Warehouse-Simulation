namespace Warehouse_Simulation_Tests.Model
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Warehouse_Simulation_Model.Model;

    [TestClass]
    public class CASCellTests
    {
        private CASCell? _testClass;
        private int _i;
        private int _j;
        private CASCell _p;
        private int _t;

        [TestInitialize]
        public void SetUp()
        {
            _i = 1170451990;
            _j = 537964413;
            _p = new CASCell(430843507, 1208397320, default(CASCell), 609786662);
            _t = 1743549481;
            _testClass = new CASCell(_i, _j, _p, _t);
        }

        [TestMethod]
        public void CanConstruct()
        {
            // Act
            var instance = new CASCell(_i, _j, _p, _t);

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CanCallSetH_F()
        {
            //valaki
        }
    }
}