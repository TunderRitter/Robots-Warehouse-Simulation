namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class representing a cell in the A* algorithm
/// </summary>
public class CASCell
{
    /// <summary>
    /// property representing the row of the cell
    /// </summary>
    public int I { get; set; }

    /// <summary>
    /// property representing the column of the cell
    /// </summary>
    public int J { get; set; }

    /// <summary>
    /// property representing the cell's F value, the sum of its G and H values
    /// </summary>
    public int F { get; set; }

    /// <summary>
    /// property representing the cell's distance from the start cell
    /// </summary>
    public int G { get; set; }

    /// <summary>
    /// property representing the cell's estimated distance from the target cell, using Manhattan distance
    /// </summary>
    public int H { get; set; }

    /// <summary>
    /// property representing the parent cell from which the robot would move to this cell
    /// </summary>
    public CASCell? Parent;

    /// <summary>
    /// property representing the time the robot would move to this cell
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    /// Constructor of the cell
    /// </summary>
    /// <param name="i">the cell's row</param>
    /// <param name="j">the cell's column</param>
    /// <param name="p">the cell's parent cell</param>
    /// <param name="t">the time the robot would move to this cell</param>
    public CASCell(int i, int j, CASCell? p, int t)
    {
        I = i; J = j;
        Time = t;
        Parent = p;
    }

    /// <summary>
    /// Function for setting the cell's H and G value
    /// </summary>
    /// <param name="i">the row of the target cell</param>
    /// <param name="j">the column of the target cell</param>
    public void SetH_F(int i, int j)
    {
        H = Math.Abs(I - i) + Math.Abs(J - j);
        F = G + H;
    }
}

/// <summary>
/// Class representing the Cooperative A* algorithm. It can calculate a robot's path while avoiding collision with previously calculated paths of other robots
/// </summary>
public class CAStar
{
    /// <summary>
    /// Property representing the current time in the algorithm
    /// </summary>
    public int TimeStep;
    /// <summary>
    /// Dictionary storing the cell reservations. They keys are tuples for the reserved cells' coordinates, and the values are the moments when that specific cell is occupied.
    /// </summary>
    private readonly Dictionary<(int I, int J), List<int>> Reservations;
    /// <summary>
    /// Dictionary storing the cell reservations, after the robot finished the task.
    /// </summary>
    private readonly Dictionary<(int I, int J), int> reservationsFinish;
    /// <summary>
    /// Variable representing the map. Walls are true, empty cells are false.
    /// </summary>
    private readonly bool[,] Map;
    /// <summary>
    /// The reserved cells for waiting robots.
    /// </summary>
    private readonly bool[,] reservationMap;
    /// <summary>
    /// Variable for the map's height
    /// </summary>
    private readonly int Row;
    /// <summary>
    /// Variable for the map's width
    /// </summary>
    private readonly int Col;

    /// <summary>
    /// Constructor of the Cooperative A* algorithm
    /// </summary>
    /// <param name="m">The map</param>
    public CAStar(bool[,] m)
    {
        TimeStep = 0;
        Reservations = [];
        reservationsFinish = [];
        Map = m;
        Row = Map.GetLength(0);
        Col = Map.GetLength(1);
        reservationMap = new bool[Row, Col];
    }

    /// <summary>
    /// Function for finding the cell with the lowest F value in the Open list: this will be the most ideal cell for the algorithm to continue the path with
    /// </summary>
    /// <param name="list">The Open list where we search for the cell</param>
    /// <returns>The index of the cell with the lowest F value in the list</returns>
    public static int LowestFCost(List<CASCell> list)
    {
        int lowest = 0;
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].F < list[lowest].F) lowest = i;
        }
        return lowest;
    }

    /// <summary>
    /// Reserve cell for waiting robot.
    /// </summary>
    /// <param name="robot"></param>
    public void Reserve(Robot robot)
    {
        reservationMap[robot.Pos.row, robot.Pos.col] = true;
        reservationsFinish.Remove(robot.Pos);
    }

    /// <summary>
    /// Remove cell reservation.
    /// </summary>
    /// <param name="robot"></param>
    public void UnReserve(Robot robot)
    {
        reservationMap[robot.Pos.row, robot.Pos.col] = false;
    }

    /// <summary>
    /// Function for deciding if the robot has been waiting for too long
    /// </summary>
    /// <param name="cell">The cell where the robot currently stands</param>
    /// <returns>true if the waiting is too long, false otherwise</returns>
    private static bool TooLongWait(CASCell cell)
    {
        int n = 1;
        while (cell.Parent != null && cell.I == cell.Parent.I && cell.J == cell.Parent.J)
        {
            n++;
            cell = cell.Parent;
        }
        return n > 5;
    }
    /// <summary>
    /// Function for calculating a robot's path
    /// </summary>
    /// <param name="Robot">The robot whose path we want to calculate</param>
    /// <param name="StartTime">The time we call the function</param>
    /// <returns>The queue containing the calculated path</returns>
    /// <exception cref="ArgumentNullException">Exception thrown if the robot has no target</exception>
    public Queue<(int, int)> FindPath(Robot Robot, int StartTime)
    {
        if (Robot.TargetPos == null)
        {
            throw new ArgumentNullException("This robot has no target");
        }

        List<CASCell> Open = [];
        List<CASCell> Closed = [];


        CASCell Start = new(Robot.Pos.row, Robot.Pos.col, null, StartTime) { G = 0 };
        Start.SetH_F(Robot.TargetPos.Value.row, Robot.TargetPos.Value.col);

        Open.Add(Start);

        while (Open.Count != 0)
        {
            int idx = LowestFCost(Open);
            CASCell Current = Open[idx];
            Open.RemoveAt(idx);
            if (Current.I == Robot.TargetPos.Value.row && Current.J == Robot.TargetPos.Value.col)
            {
                if (reservationsFinish.TryGetValue(Robot.TargetPos.Value, out int value))
                    reservationsFinish[Robot.TargetPos.Value] = Math.Min(value, Current.Time);
                else
                    reservationsFinish.Add(Robot.TargetPos.Value, Current.Time);
                return GetPath(Current);
            }
            Closed.Add(Current);

            bool neighborAdded = false;

            int time = Current.Time;

            (int I, int J)[] neighbors =
                [ (Current.I, Current.J - 1),
                  (Current.I, Current.J + 1),
                  (Current.I - 1, Current.J),
                  (Current.I + 1, Current.J) ];

            foreach ((int I, int J) Neighbor in neighbors)
            {
                if (Neighbor.I < 0 || Neighbor.I >= Row ||
                    Neighbor.J < 0 || Neighbor.J >= Col) continue;

                if (Map[Neighbor.I, Neighbor.J] ||
                    reservationMap[Neighbor.I, Neighbor.J]) continue;

                if (Current.Parent == null)
                {
                    if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) ||
                        Reservations.ContainsKey((Current.I, Current.J)))
                    {
                        // the robot goes forward
                        if ((Robot.Direction == Direction.N && Current.I > Neighbor.I) ||
                            (Robot.Direction == Direction.S && Current.I < Neighbor.I) ||
                            (Robot.Direction == Direction.E && Current.J < Neighbor.J) ||
                            (Robot.Direction == Direction.W && Current.J > Neighbor.J))
                        {
                            // the neighbor cell is occupied in the next step
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;

                            // another robot is waiting in the neighbor cell
                            if (reservationsFinish.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                reservationsFinish[(Neighbor.I, Neighbor.J)] <= time + 1 &&
                                Neighbor != Robot.TargetPos) continue;

                            // possibility of two robots switching places
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;
                        }
                        // the robot has to turn to left or right
                        else if ((Robot.Direction == Direction.N && Current.J != Neighbor.J) ||
                            (Robot.Direction == Direction.S && Current.J != Neighbor.J) ||
                            (Robot.Direction == Direction.E && Current.I != Neighbor.I) ||
                            (Robot.Direction == Direction.W && Current.I != Neighbor.I))
                        {
                            // the robot has no time to turn, the cell is occupied in the next step
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;

                            // a robot is waiting here
                            if (reservationsFinish.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                reservationsFinish[(Neighbor.I, Neighbor.J)] <= time + 2 &&
                                Neighbor != Robot.TargetPos) continue;

                            // the neighbor cell is occupied when the robot wants to step there
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 2)) continue;

                            // possibility of two eobots switching places
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 2) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;
                        }
                        // the robot has to take a 180 degree turn
                        else if ((Robot.Direction == Direction.N && Current.I < Neighbor.I) ||
                            (Robot.Direction == Direction.S && Current.I > Neighbor.I) ||
                            (Robot.Direction == Direction.E && Current.J > Neighbor.J) ||
                            (Robot.Direction == Direction.W && Current.J < Neighbor.J))
                        {
                            // the neighbor cell is occupied when the robot wants to step there
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 3)) continue;

                            //another robot is waiting there
                            if (reservationsFinish.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                reservationsFinish[(Neighbor.I, Neighbor.J)] <= time + 3 &&
                                Neighbor != Robot.TargetPos) continue;

                            // the robot has no time to turn
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 2)) continue;

                            // possibility of two robots switching places
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 3) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 2)) continue;
                        }
                    }

                }
                else
                {
                    if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) ||
                        Reservations.ContainsKey((Current.I, Current.J)))
                    {
                        // the robot moves forward
                        if ((Current.Parent.I < Current.I && Current.I < Neighbor.I) ||
                            (Current.Parent.I > Current.I && Current.I > Neighbor.I) ||
                            (Current.Parent.J < Current.J && Current.J < Neighbor.J) ||
                            (Current.Parent.J > Current.J && Current.J > Neighbor.J))
                        {
                            // the neighbor cell is occupied
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;

                            // a robot is waiting here
                            if (reservationsFinish.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                reservationsFinish[(Neighbor.I, Neighbor.J)] <= time + 1 &&
                                Neighbor != Robot.TargetPos) continue;

                            // possibility of two robots switching places
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;
                        }
                        // the robot has to turn left or right
                        if ((Current.Parent.I == Current.I && Current.I != Neighbor.I) ||
                            (Current.Parent.J == Current.J && Current.J != Neighbor.J))
                        {
                            // no time to turn
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;

                            // a robot is waiting there
                            if (reservationsFinish.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                reservationsFinish[(Neighbor.I, Neighbor.J)] <= time + 2 &&
                                Neighbor != Robot.TargetPos) continue;

                            // the cell is occupied
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 2)) continue;

                            // possibility of two robots switching places
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 2) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;
                        }
                        // the robot has to take a 180 degree turn
                        else if ((Current.Parent.I < Current.I && Current.I > Neighbor.I) ||
                            (Current.Parent.I > Current.I && Current.I < Neighbor.I) ||
                            (Current.Parent.J < Current.J && Current.J > Neighbor.J) ||
                            (Current.Parent.J > Current.J && Current.J < Neighbor.J))
                        {
                            // the cell is occupied
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 3)) continue;

                            //a robot is waiting here
                            if (reservationsFinish.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                reservationsFinish[(Neighbor.I, Neighbor.J)] <= time + 3 &&
                                Neighbor != Robot.TargetPos) continue;

                            // no time for turn
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 2)) continue;

                            // possibility of two robots switching places
                            if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 3) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 2)) continue;
                        }
                    }
                }

                // the robot can move to this neighbor cell
                int next_cost = Current.G + 1;
                int n;
                if ((n = Open.FindIndex(c => c.I == Neighbor.I && c.J == Neighbor.J)) != -1)
                {
                    if (Open[n].G > next_cost) Open.RemoveAt(n);
                }
                if ((n = Closed.FindIndex(c => c.I == Neighbor.I && c.J == Neighbor.J)) != -1)
                {
                    if (Closed[n].G > next_cost) Closed.RemoveAt(n);
                }

                if (!(Open.Any(c => c.I == Neighbor.I && c.J == Neighbor.J)) &&
                            !(Closed.Any(c => c.I == Neighbor.I && c.J == Neighbor.J)))
                {
                    int n_time = Current.Time;
                    if (Current.Parent == null)
                    {
                        if ((Robot.Direction == Direction.N && Current.I > Neighbor.I) ||
                            (Robot.Direction == Direction.S && Current.I < Neighbor.I) ||
                            (Robot.Direction == Direction.E && Current.J < Neighbor.J) ||
                            (Robot.Direction == Direction.W && Current.J > Neighbor.J))
                        {
                            n_time = Current.Time + 1;
                        }
                        else if ((Robot.Direction == Direction.N && Current.J != Neighbor.J) ||
                            (Robot.Direction == Direction.S && Current.J != Neighbor.J) ||
                            (Robot.Direction == Direction.E && Current.I != Neighbor.I) ||
                            (Robot.Direction == Direction.W && Current.I != Neighbor.I))
                        {
                            n_time = Current.Time + 2;
                        }
                        else if ((Robot.Direction == Direction.N && Current.I < Neighbor.I) ||
                            (Robot.Direction == Direction.S && Current.I > Neighbor.I) ||
                            (Robot.Direction == Direction.E && Current.J > Neighbor.J) ||
                            (Robot.Direction == Direction.W && Current.J < Neighbor.J))
                        {
                            n_time = Current.Time + 3;
                        }
                    }
                    else
                    {
                        if ((Current.Parent.I < Current.I && Current.I < Neighbor.I) ||
                            (Current.Parent.I > Current.I && Current.I > Neighbor.I) ||
                            (Current.Parent.J < Current.J && Current.J < Neighbor.J) ||
                            (Current.Parent.J > Current.J && Current.J > Neighbor.J))
                        {
                            n_time = Current.Time + 1;
                        }
                        if ((Current.Parent.I == Current.I && Current.I != Neighbor.I) ||
                            (Current.Parent.J == Current.J && Current.J != Neighbor.J))
                        {
                            n_time = Current.Time + 2;
                        }
                    }

                    CASCell NewCell = new(Neighbor.I, Neighbor.J, Current, n_time) { G = next_cost };
                    NewCell.SetH_F(Robot.TargetPos.Value.row, Robot.TargetPos.Value.col);
                    Open.Add(NewCell);
                    neighborAdded = true;
                }

            }

            if (!neighborAdded && Reservations.ContainsKey((Current.I, Current.J)) &&
                !Reservations[(Current.I, Current.J)].Contains(time + 1) && !TooLongWait(Current))
            {
                CASCell Waiting = new(Current.I, Current.J, Current, Current.Time + 1) { G = Current.G + 1 };
                Waiting.SetH_F(Robot.TargetPos.Value.row, Robot.TargetPos.Value.col);
                Open.Add(Waiting);
            }
        }
        return new Queue<(int, int)>();
    }

    /// <summary>
    /// Function for reconstructing a path from the target cell by walking back on the parent cells
    /// </summary>
    /// <param name="Cell">The last/target cell of the path</param>
    /// <returns>The queue containing the path</returns>
    public Queue<(int, int)> GetPath(CASCell? Cell)
    {
        List<(int, int)> path = [];
        while (Cell != null)
        {
            path.Insert(0, (Cell.I, Cell.J));
            if (!Reservations.ContainsKey((Cell.I, Cell.J)))
                Reservations.Add((Cell.I, Cell.J), []);
            Reservations[(Cell.I, Cell.J)].Add(Cell.Time);



            if (Cell.Parent != null)
            {
                if (!Reservations.ContainsKey((Cell.Parent.I, Cell.Parent.J)))
                    Reservations.Add((Cell.Parent.I, Cell.Parent.J), []);
                for (int i = Cell.Parent.Time + 1; i < Cell.Time; i++)
                {
                    Reservations[(Cell.Parent.I, Cell.Parent.J)].Add(i);
                }
            }
            Cell = Cell.Parent;
        }

        Queue<(int, int)> Path = new(path);
        Path.Dequeue();
        return Path;
    }
}