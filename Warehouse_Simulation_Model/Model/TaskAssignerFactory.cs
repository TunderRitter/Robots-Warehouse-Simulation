namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class that creates the task assigners.
/// </summary>
public static class TaskAssignerFactory
{
    #region Methods
    /// <summary>
    /// Method that creates the task assigners.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static ITaskAssigner Create(string type)
    {
        return type switch
        {
            "roundrobin" => new RoundRobin(),
            _ => throw new InvalidDataException("Not supported task assigner"),
        };
    }
    #endregion
}
