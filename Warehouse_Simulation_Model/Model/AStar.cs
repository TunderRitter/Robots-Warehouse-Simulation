using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model.Model;

public class AStarCell
{
    public int I, J;
    public AStarCell? Parent;
    // g = távolság a kiindulóponttól
    // h = távolság a célállomástól (légvonalban, falakat nem figyelve)
    // f = g + h
    public int f, g, h;

    public AStarCell(int i, int j, AStarCell? parent)
    {
        I = i; J = j;
        Parent = parent;
        f = 0; g = 0; h = 0;
    }
}
public class AStar
{

}
