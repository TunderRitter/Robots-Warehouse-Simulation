namespace Warehouse_Simulation_Model.Model;


public class RoundRobin : ITaskAssigner
{
    public void Assign(Robot[] robots, Target[] targets)
    {
        if (robots.Length != targets.Length) throw new ArgumentException("Robots and targets are different size");

        for (int i = 0; i < robots.Length; i++)
        {
            robots[i].TargetPos = targets[i].Pos;
            targets[i].Id = robots[i].Id;
        }
    }
}
