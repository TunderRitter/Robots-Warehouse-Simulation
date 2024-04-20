namespace Warehouse_Simulation_Model.Model;


public class Controller
{
    private readonly Robot[] _robots;
    private readonly Queue<(int, int)>[] _routes;
    private AStar _astar;
    private CAStar _castar;
    public int step { get; set; }


    public Controller(bool[,] map, Robot[] robots)
    {
        _astar = new AStar(map);
        _castar = new CAStar(map);
        _robots = robots;
        _routes = new Queue<(int, int)>[robots.Length];
        for (int i = 0; i < robots.Length; i++)
        {
            _routes[i] = new Queue<(int, int)>();
        }
        step = 0;
    }


    public void CalculateRoutes()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            if (_robots[i].TargetPos != null && _routes[i].Count == 0)
                //_routes[i] = _astar.AStarSearch(_robots[i]);
                _routes[i] = _castar.FindPath(_robots[i], step);
        }
    }

    public string[] CalculateSteps()
    {
        string[] steps = new string[_robots.Length];
        for (int i = 0; i < _robots.Length; i++)
        {
            steps[i] = CalculateStep(_robots[i], _routes[i]);
        }
        return steps;
    }

    private static string CalculateStep(Robot robot, Queue<(int, int)> route)
    {
        if (route.Count == 0) return "W";
        
        (int row, int col) posTo = route.Peek();
        (int row, int col) posFrom = robot.Pos;

        string move = "";

        if (posFrom == posTo)
        {
            move = "W";
            route.Dequeue();
        }
        else if (posFrom.row == posTo.row)
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

                //if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
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

                //if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
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

                //if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
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

                //if (!_passThrough && move == "F" && Map[posTo.row, posTo.col] is Floor floor && floor.Robot != null) move = "W";
            }
        }

        if (move == "F")
            route.Dequeue();

        return move;
    }



    // HANDLE DUPLICATE COORDINATES
    private static string CalculateStep2(Robot robot, Queue<(int, int)> route)
    {
        (int row, int col) posFrom = robot.Pos;
        (int row, int col)? posTo = route.ElementAtOrDefault(0);

        string move = "";
        int wait = 0;

        if (posTo == null)
        {
            move = "W";
        }
        else if (posFrom == posTo)
        {
            posTo = route.ElementAtOrDefault(1);
            if (posTo == null)
            {
                move = "W";
            }
            else if (posFrom == posTo)
            {
                posTo = route.ElementAtOrDefault(2);
                wait = 2;
                if (posTo == null || posFrom == posTo)
                {
                    move = "W";
                }
            }
            else
            {
                wait = 1;
            }
        }
        if (move == "W") return move;

        if (posFrom.row == posTo?.row)
        {
            if (posFrom.col - 1 == posTo?.col)
            {
                move = robot.Direction switch
                {
                    Direction.N => "C",
                    Direction.E => "R",
                    Direction.S => "R",
                    Direction.W => "F",
                    _ => throw new Exception(),
                };

                if (move == "R" && wait != 2)
                    move = "W";
                else if ((move == "R" || move == "C") && wait != 1)
                    move = "W";
            }
            else if (posFrom.col + 1 == posTo?.col)
            {
                move = robot.Direction switch
                {
                    Direction.N => "R",
                    Direction.E => "F",
                    Direction.S => "C",
                    Direction.W => "R",
                    _ => throw new Exception(),
                };

                if (move == "R" && wait != 2)
                    move = "W";
                else if ((move == "R" || move == "C") && wait != 1)
                    move = "W";
            }
        }
        else if (posFrom.col == posTo?.col)
        {
            if (posFrom.row - 1 == posTo?.row)
            {
                move = robot.Direction switch
                {
                    Direction.N => "F",
                    Direction.E => "C",
                    Direction.S => "R",
                    Direction.W => "R",
                    _ => throw new Exception(),
                };

                if (move == "R" && wait != 2)
                    move = "W";
                else if ((move == "R" || move == "C") && wait != 1)
                    move = "W";
            }
            else if (posFrom.row + 1 == posTo?.row)
            {
                move = robot.Direction switch
                {
                    Direction.N => "R",
                    Direction.E => "R",
                    Direction.S => "F",
                    Direction.W => "C",
                    _ => throw new Exception(),
                };

                if (move == "R" && wait != 2)
                    move = "W";
                else if ((move == "R" || move == "C") && wait != 1)
                    move = "W";
            }
        }

        if (move != "W")
            route.Dequeue();

        return move;
    }
}
