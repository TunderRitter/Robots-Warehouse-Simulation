using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.persistence
{
    public class Log
    {
        public string actionModel;
        public string AllValid;
        public int teamSize;
        public List<object[]> start;
        public int numTaskFinished;
        public int sumOfCost;
        public int MakeSpan;
        public List<string> actualPaths;
        public List<string> plannerPaths;
        public List<object[]> errors;
        public List<object[]> events;
        public List<int[]> tasks;

        public Log()
        {
            actionModel = "";
            AllValid = "";
            teamSize = 0;
            start = new List<object[]>();
            numTaskFinished = 0;
            sumOfCost = 0;
            MakeSpan = 0;
            actualPaths = new List<string>();
            plannerPaths = new List<string>();
            errors = new List<object[]>();
            events = new List<object[]>();
            tasks = new List<int[]>();
        }

        public Log Read(string str)
        {
            return new Log();
        }

        public Log Write(string str)
        {
            return new Log();
        }
    }
}
