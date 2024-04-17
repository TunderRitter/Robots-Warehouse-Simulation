using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.Model;

public class CASCell
{
    public int I;
    public int J;
    /*
     * g: A kiindulóponttól megtett távolság
     * h: becsült hátralévő távolság a célállomástól (a sor- és oszlopindexek különbségeinek összege)
     * f: g + h, ez alapján választjuk ki az Open listából a legkedvezőbb cellát
     */
    public int f, g, h;

    public CASCell? Parent;

    //annak az időpillanata, hogy a cellába léptünk
    public int Time;

    public CASCell(int i, int j, CASCell? p, int t)
    {
        I = i; J = j;
        Time = t;
        Parent = p;
    }
    public void SetH_F(int i, int j)
    {
        h = Math.Abs(I - i) + Math.Abs(J - j);
        f = g + h;
    }
}

public class CAStar
{
    public int TimeStep;
    /// <summary>
    /// Adott (i,j) cella mely időpillanatokban lett már lefoglalva
    /// </summary>
    private Dictionary<(int I, int J), List<int>> Reservations;
    private bool[,] Map;
    private int Row;
    private int Col;

    public CAStar(bool[,] m)
    {
        TimeStep = 0;
        Reservations = new();
        Map = m;
        Row = Map.GetLength(0);
        Col = Map.GetLength(1);
    }

    public static int Lowest_f_cost(List<CASCell> list)
    {
        int lowest = 0;
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].f < list[lowest].f) lowest = i;
        }
        return lowest;
    }
    public Queue<(int, int)> FindPath(Robot Robot, int StartTime)
    {
        if (Robot.TargetPos == null)
        {
            throw new ArgumentNullException("This robot has no target");
        }

        List<CASCell> Open = [];
        List<CASCell> Closed = [];


        CASCell Start = new CASCell(Robot.Pos.row, Robot.Pos.col, null, StartTime)
        {
            g = 0
        };
        Start.SetH_F(Robot.TargetPos.Value.row, Robot.TargetPos.Value.col);

        Open.Add(Start);

        while (!(Open.Count() == 0))
        {
            int idx = Lowest_f_cost(Open);
            CASCell Current = Open[idx];
            Open.RemoveAt(idx);
            if (Current.I == Robot.TargetPos.Value.row && Current.J == Robot.TargetPos.Value.col)
            {
                return GetPath(Current);
            }
            Closed.Add(Current);

            bool neighbor_added = false;

            int time = Current.Time;

            (int I, int J)[] neighbors =
                { (Current.I, Current.J - 1),
                  (Current.I, Current.J + 1),
                  (Current.I - 1, Current.J),
                  (Current.I + 1, Current.J) };

            foreach ((int I, int J) Neighbor in neighbors)
            {
                if (Neighbor.I < 0 || Neighbor.I >= Row ||
                    Neighbor.J < 0 || Neighbor.J >= Col) continue;

                if (Map[Neighbor.I, Neighbor.J]) continue;

                if (Current.Parent == null)
                {
                    if ((Robot.Direction == Direction.N && Current.I > Neighbor.I) ||
                        (Robot.Direction == Direction.S && Current.I < Neighbor.I) ||
                        (Robot.Direction == Direction.E && Current.J < Neighbor.J) ||
                        (Robot.Direction == Direction.W && Current.J > Neighbor.J))
                    {
                        if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                            Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;
                    }
                    else if ((Robot.Direction == Direction.N && Current.J != Neighbor.J) ||
                        (Robot.Direction == Direction.S && Current.J != Neighbor.J) ||
                        (Robot.Direction == Direction.E && Current.I != Neighbor.I) ||
                        (Robot.Direction == Direction.W && Current.I != Neighbor.I))
                    {
                        if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                            Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 2)) continue;
                    }
                    else if ((Robot.Direction == Direction.N && Current.I < Neighbor.I) ||
                        (Robot.Direction == Direction.S && Current.I > Neighbor.I) ||
                        (Robot.Direction == Direction.E && Current.J > Neighbor.J) ||
                        (Robot.Direction == Direction.W && Current.J < Neighbor.J))
                    {
                        if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)) &&
                            Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 3)) continue;
                    }
                }
                else
                {
                    // Le van foglalva a szomszédos cella, lehet hogy nem jó:
                    if (Reservations.ContainsKey((Neighbor.I, Neighbor.J)))
                    {
                        //egyenesen lép tovább, nem kell fordulni
                        if ((Current.Parent.I == Current.I && Current.I == Neighbor.I) ||
                        (Current.Parent.J == Current.J && Current.J == Neighbor.J))
                        {
                            //foglalt a következő lépés idejében a cella
                            if (Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;

                            if (Reservations[(Neighbor.I, Neighbor.J)].Contains(time)) continue;

                            //most foglalt a cella, valszeg helycsere lenne
                            if (Reservations[(Neighbor.I, Neighbor.J)].Contains(time) &&
                                Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;
                        }

                        //Jobbra/balra lép tovább, fordulnia kell egyet
                        if ((Current.Parent.I == Current.I && Current.I != Neighbor.I) ||
                            (Current.Parent.J == Current.J && Current.J != Neighbor.J))
                        {
                            //a következő időegységben foglalt a cellánk, nincs időnk megfordulni
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1)) continue;

                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 1))

                                //két időegység múlva foglalt a szomszéd cella
                                if (Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 2)) continue;

                            //két időegység múlva foglalt a mi cellánk, egy időegység múlva foglalt a szomszéd:
                            //helycsere esélye
                            if (Reservations.ContainsKey((Current.I, Current.J)) &&
                                Reservations[(Current.I, Current.J)].Contains(time + 2) &&
                                Reservations[(Neighbor.I, Neighbor.J)].Contains(time + 1)) continue;
                        }
                    }
                }

                //esélyes a cella arra hogy odalépjünk
                int next_cost = Current.g + 1;
                int n;
                if ((n = Open.FindIndex(c => c.I == Neighbor.I && c.J == Neighbor.J)) != -1)
                {
                    if (Open[n].g > next_cost) Open.RemoveAt(n);
                }
                if ((n = Closed.FindIndex(c => c.I == Neighbor.I && c.J == Neighbor.J)) != -1)
                {
                    if (Closed[n].g > next_cost) Closed.RemoveAt(n);
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
                        if ((Current.Parent.I == Current.I && Current.I == Neighbor.I) ||
                        (Current.Parent.J == Current.J && Current.J == Neighbor.J))
                        {
                            n_time = Current.Time + 1;
                        }
                        if ((Current.Parent.I == Current.I && Current.I != Neighbor.I) ||
                            (Current.Parent.J == Current.J && Current.J != Neighbor.J))
                        {
                            n_time = Current.Time + 2;
                        }
                    }
                    CASCell NewCell = new CASCell(Neighbor.I, Neighbor.J, Current, n_time);
                    NewCell.g = next_cost;
                    NewCell.SetH_F(Robot.TargetPos.Value.row, Robot.TargetPos.Value.col);
                    Open.Add(NewCell);
                    neighbor_added = true;
                }

            }

            /*if (!neighbor_added)
            {
                CASCell Waiting = new CASCell(Current.I, Current.J, Current, Current.Time + 1);
                Waiting.g = Current.g + 1;
                Waiting.SetH_F(Robot.TargetPos.Value.row, Robot.TargetPos.Value.col);
                Open.Add(Waiting);
            }*/
        }
        return new Queue<(int, int)>();
    }
    public Queue<(int, int)> GetPath(CASCell? Cell)
    {
        List<(int, int)> path = [];
        while (Cell != null)
        {
            path.Insert(0, (Cell.I, Cell.J));
            if (!Reservations.ContainsKey((Cell.I, Cell.J)))
                Reservations.Add((Cell.I, Cell.J), new List<int>());
            if (Reservations[(Cell.I, Cell.J)].Contains(Cell.Time))
            {
                Debug.WriteLine("már foglalt!!!!");
            }
            Reservations[(Cell.I, Cell.J)].Add(Cell.Time);



            if (Cell.Parent != null)
            {
                if (!Reservations.ContainsKey((Cell.Parent.I, Cell.Parent.J)))
                    Reservations.Add((Cell.Parent.I, Cell.Parent.J), new List<int>());
                for (int i = Cell.Parent.Time + 1; i < Cell.Time; i++)
                {
                    if (Reservations[(Cell.Parent.I, Cell.Parent.J)].Contains(i))
                    {
                        Debug.WriteLine("már foglalt!!!!");
                    }
                    Reservations[(Cell.Parent.I, Cell.Parent.J)].Add(i);
                }
            }
            Cell = Cell.Parent;
        }

        Queue<(int, int)> Path = new Queue<(int, int)>(path);
        Path.Dequeue();
        return Path;
    }
}