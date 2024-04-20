namespace Warehouse_Simulation_Model.Model;


public interface ITaskAssigner
{
    public (int, int)[] Assign(List<Robot> robots, List<Target> targets);
}
