namespace Warehouse_Simulation_Model.Model;


public interface ITaskAssigner
{
    public void Assign(Robot[] robots, Target[] targets);
}
