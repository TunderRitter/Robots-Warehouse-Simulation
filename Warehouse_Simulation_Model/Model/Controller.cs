using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.Model
{
    public class Controller
    {
        AStar _astar;
        public Controller(AStar astar)
        {
            _astar = astar;
        }

        public (int, String) CalculateStep(Robot robot, Queue<(int, int)> route, bool _passThrough, Cell[,] Map, int i)
        {
            (int row, int col) posTo = route.Peek();
            (int row, int col) posFrom = robot.Pos;

            string move = "";

            if (posFrom.row == posTo.row)
            {
                if (posFrom.col - 1 == posTo.col)
                {
                    move = robot.Direction switch
                    {
                        Direction.N => "C",
                        Direction.E => "R",
                        Direction.S => "R",
                        Direction.W => "F",
                        _ => throw new Exception(),
                    };

                    if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
                }
                else if (posFrom.col + 1 == posTo.col)
                {
                    move = robot.Direction switch
                    {
                        Direction.N => "R",
                        Direction.E => "F",
                        Direction.S => "C",
                        Direction.W => "R",
                        _ => throw new Exception(),
                    };

                    if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
                }
            }
            else if (posFrom.col == posTo.col)
            {
                if (posFrom.row - 1 == posTo.row)
                {
                    move = robot.Direction switch
                    {
                        Direction.N => "F",
                        Direction.E => "C",
                        Direction.S => "R",
                        Direction.W => "R",
                        _ => throw new Exception(),
                    };

                    if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
                }
                else if (posFrom.row + 1 == posTo.row)
                {
                    move = robot.Direction switch
                    {
                        Direction.N => "R",
                        Direction.E => "R",
                        Direction.S => "F",
                        Direction.W => "C",
                        _ => throw new Exception(),
                    };

                    if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
                }
            }

            return (i, move);
        }

        public Queue<(int, int)> CalculateRoutes(Robot robot)
        {
            return _astar.AStarSearch(robot);
        }
    }
}
