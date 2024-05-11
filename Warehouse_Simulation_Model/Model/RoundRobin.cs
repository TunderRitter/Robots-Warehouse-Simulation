namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class that assigns the robots to the targets in a round-robin fashion.
/// </summary>
public class RoundRobin : ITaskAssigner
{
    #region Methods
    /// <summary>
    /// Method that assigns the robots to the targets in a round-robin fashion.
    /// </summary>
    /// <param name="robots"></param>
    /// <param name="targets"></param>
    /// <returns></returns>
    public (int, int)[] Assign(List<Robot> robots, List<Target> targets)
    {
        List<(int, int)> assignments = [];
        int len = Math.Min(robots.Count, targets.Count);
        for (int i = 0; i < len; i++)
        {
            robots[i].TargetPos = targets[i].Pos;
            targets[i].Id = robots[i].Id;
            assignments.Add((robots[i].Id, targets[i].InitId));
        }

        return [.. assignments];
    }
    #endregion
}
