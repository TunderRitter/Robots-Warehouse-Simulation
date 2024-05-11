namespace Warehouse_Simulation_Model.Model;

/// <summary>
/// Class representing a robot in the warehouse.
/// </summary>
public class Robot
{
    #region Properties
    /// <summary>
    /// Property representing the id of the robot.
    /// </summary>
    public int Id { get; init; }
    /// <summary>
    /// Property representing the position of the robot.
    /// </summary>
    public (int row, int col) Pos { get; private set; }
    /// <summary>
    /// Property representing the target position of the robot.
    /// </summary>
    public (int row, int col)? TargetPos { get; set; }
    /// <summary>
    /// Property representing the direction of the robot.
    /// </summary>
    public Direction Direction { get; private set; }
    #endregion

    #region Events
    /// <summary>
    /// Event for when the robot reaches its target.
    /// </summary>
    public event EventHandler? Finished;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="Robot"/> class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <param name="direction"></param>
    public Robot(int id, (int, int) pos, Direction direction)
    {
        Id = id;
        Pos = pos;
        TargetPos = null;
        Direction = direction;
    }

    /// <summary>
    /// Method for calculating the next move of the robot.
    /// </summary>
    /// <returns> Position of the result of the move. </returns>
    /// <exception cref="Exception"></exception>
    public (int row, int col) NextMove()
    {
        return Direction switch
        {
            Direction.N => (Pos.row - 1, Pos.col),
            Direction.E => (Pos.row, Pos.col + 1),
            Direction.S => (Pos.row + 1, Pos.col),
            Direction.W => (Pos.row, Pos.col - 1),
            _ => throw new Exception(),
        };
    }

    /// <summary>
    /// Method for moving the robot.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Move()
    {
        Pos = Direction switch
        {
            Direction.N => (Pos.row - 1, Pos.col),
            Direction.E => (Pos.row, Pos.col + 1),
            Direction.S => (Pos.row + 1, Pos.col),
            Direction.W => (Pos.row, Pos.col - 1),
            _ => throw new Exception(),
        };
    }

    /// <summary>
    /// Method for turning the robot right.
    /// </summary>
    public void TurnRight()
    {
        switch(Direction)
        {
            case Direction.N:
                Direction = Direction.E;
                break;
            case Direction.E:
                Direction = Direction.S;
                break;
            case Direction.S:
                Direction = Direction.W;
                break;
            case Direction.W:
                Direction = Direction.N;
                break;
        }
    }

    /// <summary>
    /// Method for turning the robot left.
    /// </summary>
    public void TurnLeft()
    {
        switch (Direction)
        {
            case Direction.N:
                Direction = Direction.W;
                break;
            case Direction.E:
                Direction = Direction.N;
                break;
            case Direction.S:
                Direction = Direction.E;
                break;
            case Direction.W:
                Direction = Direction.S;
                break;
        }
    }

    /// <summary>
    /// Method for checking if the robot has reached its target.
    /// </summary>
    public void CheckPos()
    {
        if (Pos == TargetPos)
            OnFinished();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Method for when the robot reaches its target.
    /// </summary>
    private void OnFinished()
    {
        TargetPos = null;
        Finished?.Invoke(this , EventArgs.Empty);
    }
    #endregion
}
