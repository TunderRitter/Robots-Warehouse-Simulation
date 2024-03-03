namespace Warehouse_Simulation_Model.Model;


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
