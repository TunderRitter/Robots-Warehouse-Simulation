using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse_Simulation_Model;


public abstract class Cell { }


public class Wall : Cell { }


public class  Floor : Cell
{
    public Robot? Robot { get; set; }
    public Target? Target { get; set; }

    public Floor(Robot? robot = null, Target? target = null)
    {
        Robot = robot;
        Target = target;
    }
}
