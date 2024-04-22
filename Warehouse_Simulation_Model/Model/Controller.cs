namespace Warehouse_Simulation_Model.Model;


public class Controller
{
    private readonly Robot[] _robots;
    private readonly Queue<(int, int)>[] _routes;
    private bool[] _reserved;
    private int[] _stuck;
    private readonly bool[,] _map;
    //private AStar _astar;
    private CAStar _castar;
    public int Step { get; set; }

    public event EventHandler<int>? RobotStuck;


    public Controller(bool[,] map, Robot[] robots)
    {
        //_astar = new AStar(map);
        _map = map;
        _castar = new CAStar(_map);
        _robots = robots;
        _routes = new Queue<(int, int)>[robots.Length];
        _reserved = new bool[robots.Length];
        _stuck = new int[robots.Length];
        for (int i = 0; i < robots.Length; i++)
        {
            _routes[i] = new Queue<(int, int)>();
        }
        Step = 0;
    }


    public void CalculateRoutes()
    {
        for (int i = 0; i < _robots.Length; i++)
        {
            if (!_reserved[i] && _routes[i].Count == 0)
            {
                _reserved[i] = true;
                _castar.Reserve(_robots[i]);
            }
        }

        for (int i = 0; i < _robots.Length; i++)
        {
            if (_robots[i].TargetPos != null && _routes[i].Count == 0)
                //_routes[i] = _astar.AStarSearch(_robots[i]);
                _routes[i] = _castar.FindPath(_robots[i], Step);
        }
    }

    public string[] CalculateSteps()
    {
        string[] steps = new string[_robots.Length];
        for (int i = 0; i < _robots.Length; i++)
        {
            steps[i] = CalculateStep(i);
        }

        HashSet<(int, int)> positions = [];
        for (int i = 0; i < _robots.Length; i++)
        {
            if (steps[i] == "F")
                positions.Add(_robots[i].NextMove());
            else
                positions.Add(_robots[i].Pos);
        }
        if (positions.Count != _robots.Length)
        {
            Array.ForEach(_routes, e => e.Clear());
            _reserved = new bool[_robots.Length];
            _castar = new CAStar(_map);

            return ["S"];
        }

        return steps;
    }

    private string CalculateStep(int idx)
    {
        Robot robot = _robots[idx];
        Queue<(int, int)> route = _routes[idx];

        if (route.Count == 0)
        {
            _stuck[idx]++;
            if (_stuck[idx] >= 5)
            {
                _stuck[idx] = 0;
                RobotStuck?.Invoke(this, idx);
            }
            return "W";
        }
        
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
            }
        }

        if (move == "F")
        {
            _castar.UnReserve(robot);
            _reserved[robot.Id] = false;
            route.Dequeue();
        }

        if (move == "W")
            _stuck[idx]++;
        else
            _stuck[idx] = 0;

        if (_stuck[idx] >= 5)
        {
            _stuck[idx] = 0;
            RobotStuck?.Invoke(this, idx);
        }

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
