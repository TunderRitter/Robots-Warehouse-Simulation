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