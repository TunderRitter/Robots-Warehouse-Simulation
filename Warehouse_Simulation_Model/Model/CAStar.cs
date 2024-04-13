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

    
}