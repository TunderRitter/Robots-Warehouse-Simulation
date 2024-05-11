namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class representing a grid in the warehouse.
/// </summary>
public abstract class Cell { }

/// <summary>
/// Class representing a wall in the warehouse.
/// </summary>
public class Wall : Cell { }

/// <summary>
/// Class representing a floor in the warehouse wich can have a robot and a target.
/// </summary>
public class  Floor : Cell
{
    #region Properties
    /// <summary>
    /// Property representing the robot on the floor.
    /// </summary>
    public Robot? Robot { get; set; }
    /// <summary>
    /// Property representing the target on the floor.
    /// </summary>
    public Target? Target { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Floor"/> class.
    /// </summary>
    /// <param name="robot"></param>
    /// <param name="target"></param>
    public Floor(Robot? robot = null, Target? target = null)
    {
        Robot = robot;
        Target = target;
    }
    #endregion
}
