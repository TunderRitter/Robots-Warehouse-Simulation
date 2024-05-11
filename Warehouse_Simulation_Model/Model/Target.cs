namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class representing a target in the warehouse.
/// </summary>
public class Target
{
    #region Properties
    /// <summary>
    /// Property representing the id the robot assigned to the target.
    /// </summary>
    public int? Id { get; set; }
    /// <summary>
    /// Property representing the position of the target.
    /// </summary>
    public (int row, int col) Pos { get; init; }
    /// <summary>
    /// Property representing if the target is active.
    /// </summary>
    public bool Active { get; set; }
    /// <summary>
    /// Property representing the id of the target.
    /// </summary>
    public int InitId { get; init; }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Target"/> class.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="initId"></param>
    public Target((int, int) pos, int initId)
    {
        Pos = pos;
        Active = false;
        InitId = initId;
    }
    #endregion
}
