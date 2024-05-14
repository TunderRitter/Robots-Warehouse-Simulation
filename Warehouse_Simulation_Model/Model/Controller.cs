namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class responsible for calculating the routes of the robots.
/// </summary>
public class Controller
{
    #region Fields
    /// <summary>
    /// Array containing the robots.
    /// </summary>
    private readonly Robot[] _robots;
    /// <summary>
    /// Array containing the routes of the robots.
    /// </summary>
    private readonly Queue<(int, int)>[] _routes;
    /// <summary>
    /// Array containing the reserved coordinates.
    /// </summary>
    private bool[] _reserved;
    /// <summary>
    /// Array containing the stuck robots.
    /// </summary>
    private int[] _stuck;
    /// <summary>
    /// Array containing the map of the warehouse.
    /// </summary>
    private readonly bool[,] _map;
    /// <summary>
    /// AStar object for calculating the routes.
    /// </summary>
    private CAStar _castar;
    #endregion

    #region Properties
    /// <summary>
    /// Property representing the current step.
    /// </summary>
    public int Step { get; set; }
    #endregion

    #region Events
    /// <summary>
    /// Event for when a robot is stuck.
    /// </summary>
    public event EventHandler<int>? RobotStuck;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Controller"/> class.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="robots"></param>
    public Controller(bool[,] map, Robot[] robots)
    {
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

    /// <summary>
    /// Calculates the routes for the robots.
    /// </summary>
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
                _routes[i] = _castar.FindPath(_robots[i], Step);
        }
    }

    /// <summary>
    /// Calculates the steps for the robots.
    /// </summary>
    /// <returns> An array conatining the next step for each robot. </returns>
    public string[] CalculateSteps()
    {
        string[] steps = new string[_robots.Length];
        for (int i = 0; i < _robots.Length; i++)
        {
            steps[i] = CalculateStep(i);
        }
        return steps;
    }

    /// <summary>
    /// Resets the pathfinding in case of failure
    /// </summary>
    public void Reset()
    {
        Array.ForEach(_routes, e => e.Clear());
        _reserved = new bool[_robots.Length];
        _stuck = new int[_robots.Length];
        _castar = new CAStar(_map);
    }

    /// <summary>
    /// Gets the route of the robot.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns> List of coordinates that is the route of the robot. </returns>
    public List<(int, int)> GetRoute(int idx)
    {
        return [_robots[idx].Pos, .. _routes[idx]];
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Calculates the step for the robot.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns> The next step for the robot. </returns>
    /// <exception cref="Exception"></exception>
    private string CalculateStep(int idx)
    {
        Robot robot = _robots[idx];
        Queue<(int, int)> route = _routes[idx];

        if (robot.TargetPos == null) return "W";

        if (route.Count == 0)
        {
            _stuck[idx]++;
            if (_stuck[idx] >= 10)
            {
                bool[] longStuck = _stuck.Select(e => e >= 5).ToArray();
                for (int i = 0; i < _stuck.Length; i++)
                {
                    if (longStuck[i])
                        RobotStuck?.Invoke(this, i);
                }
                Array.Fill(_stuck, 0);
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

        if (_stuck[idx] >= 10)
        {
            bool[] longStuck = _stuck.Select(e => e >= 5).ToArray();
            for (int i = 0; i < _stuck.Length; i++)
            {
                if (longStuck[i])
                    RobotStuck?.Invoke(this, i);
            }
            Array.Fill(_stuck, 0);
        }

        return move;
    }
    #endregion
}
