namespace Warehouse_Simulation_Model.Model;


public class RoundRobin : ITaskAssigner
{
    public void Assign(List<Robot> robots, List<Target> targets)
    {
        int len = Math.Min(robots.Count, targets.Count);
        for (int i = 0; i < len; i++)
        {
            robots[i].TargetPos = targets[i].Pos;
            targets[i].Id = robots[i].Id;
        }
    }
}
