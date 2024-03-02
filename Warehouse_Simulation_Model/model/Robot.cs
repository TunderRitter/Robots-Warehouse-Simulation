using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model
{
    public class Robot
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

        private (int, int) _targetPos;
        public (int, int) TargetPos
        {
            get { return _targetPos; }
            set { _targetPos = value; }
        }

        private Direction _direcrion;
        public Direction Direction
        {
            get { return _direcrion; }
            set { _direcrion = value; }
        }

        public Robot(int id, (int, int) pos, (int, int) targetPos, Direction direction)
        {
            Id = id;
            Pos = pos;
            TargetPos = targetPos;
            Direction = direction;
        }

        public void Turn(String dir)
        {

        }

        //+Event
    }
}
