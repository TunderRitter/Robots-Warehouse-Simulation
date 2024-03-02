using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model
{
    public class Target
    {
		private int _id;
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private (int, int) _pos;
		public (int, int) Pos
		{
			get { return _pos; }
			set { _pos = value; }
		}

		public Target(int id, (int, int) pos)
		{
            Id = id;
            Pos = pos;
        }
	}
}
