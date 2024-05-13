namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Interface for task assigning methods.
/// </summary>
public interface ITaskAssigner
{
    #region Methods
    /// <summary>
    /// Method that assigns the robots to the targets.
    /// </summary>
    /// <param name="robots"></param>
    /// <param name="targets"></param>
    /// <returns>The assigned robot and target pairs.</returns>
    public (int, int)[] Assign(List<Robot> robots, List<Target> targets);
    #endregion
}
