namespace Warehouse_Simulation_Model.Model;


public interface ITaskAssigner
{
    public void Assign(List<Robot> robots, List<Target> targets);
}
