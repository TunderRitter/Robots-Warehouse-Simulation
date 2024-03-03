using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model;


public class Target
{
    private int? _id;
    public int? Id
    {
        get => _id;
        set => _id ??= value;
    }
    public (int row, int col) Pos { get; init; }


    public Target((int, int) pos)
    {
        Pos = pos;
    }
}
