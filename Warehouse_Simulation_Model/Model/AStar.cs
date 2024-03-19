using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.Model;

public class AStarCell
{
    public int I {get; set;}
    public int J { get; set; }
    public AStarCell? Parent { get; set; }
    // g = távolság a kiindulóponttól
    // h = távolság a célállomástól (légvonalban, falakat nem figyelve)
    // f = g + h
    public int g { get; set; }
    public int h { get; set; }
    public int f { get; set; }


    public AStarCell(int i, int j, AStarCell? parent)
    {
        I = i; J = j;
        Parent = parent;
        f = 0; g = 0; h = 0;
    }
}
public class AStar
{
    private bool[,] _map;
    public AStar(bool[,] m)
    {
        _map = m;
    }
    public static int Lowest_f_cost(List<AStarCell> list)
    {
        int lowest = 0;
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].f < list[lowest].f)
            {
                lowest = i;
            }
        }
        return lowest;
    }
    public Queue<(int,int)> AStarSearch(Robot robot)
    {
        int row = _map.GetLength(0);
        int col = _map.GetLength(1);

        List<AStarCell> Open = new();
        List<AStarCell> Closed = new();

        AStarCell startcell = new AStarCell(robot.Pos.row, robot.Pos.col, null);

        Open.Add(startcell);

        AStarCell? Last;

        while (true)
        {
            int l_index = Lowest_f_cost(Open);

            AStarCell Current = Open[l_index];
            Last = Current;

            Open.RemoveAt(l_index);

            Closed.Add(Current);

            if (robot.TargetPos == null)
            {
                throw new ArgumentNullException("This poor robot has no target :(");
            }
            if (Current.I == robot.TargetPos.Value.row && Current.J == robot.TargetPos.Value.col)
            {
                break;
            }

            (int I, int J)[] neighbors =
                { (Current.I, Current.J - 1),
                  (Current.I, Current.J + 1),
                  (Current.I - 1, Current.J),
                  (Current.I + 1, Current.J) };

            foreach ((int I,int J) neighbor in neighbors)
            {
                if (!(neighbor.I < 0 || neighbor.I >= row ||
                    neighbor.J < 0 || neighbor.J >= col))
                {
                    //megnézi, hogy padló-e
                    if (!_map[neighbor.I, neighbor.J])
                    {
                        
                        //int cost = Math.Abs(neighbor.I - startcell.I) + Math.Abs(neighbor.J - startcell.J) + 1;
                        int cost = Current.g + 1;
                        int idx;
                        if ((idx = Open.FindIndex(c => c.I == neighbor.I && c.J == neighbor.J)) != -1)
                        {
                            if (Open[idx].g > cost)
                            {
                                Open.RemoveAt(idx);
                            }
                        }
                        if ((idx = Closed.FindIndex(c => c.I == neighbor.I && c.J == neighbor.J)) != -1)
                        {
                            if (Closed[idx].g > cost)
                            {
                                Closed.RemoveAt(idx);
                            }
                        }
                        if (!(Open.Any(c => c.I == neighbor.I && c.J == neighbor.J)) &&
                            !(Closed.Any(c => c.I == neighbor.I && c.J == neighbor.J)))
                        {
                            AStarCell NeighborCell = new AStarCell(neighbor.I, neighbor.J, Current);
                            NeighborCell.g = cost;
                            NeighborCell.h = Math.Abs(NeighborCell.I - robot.TargetPos.Value.row) + Math.Abs(NeighborCell.J - robot.TargetPos.Value.col);
                            NeighborCell.f = NeighborCell.g + NeighborCell.h;
                            Open.Add(NeighborCell);
                        }
                    }
                }
            }
        }

        List<(int,int)> Path_list = new();
        while (Last != null)
        {
            Path_list.Insert(0,(Last.I, Last.J));
            Last = Last.Parent;
        }
        Queue<(int, int)> Path = new Queue<(int, int)>(Path_list);
        return Path;

    }
}
