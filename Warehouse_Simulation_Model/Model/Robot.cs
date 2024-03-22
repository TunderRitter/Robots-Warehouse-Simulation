namespace Warehouse_Simulation_Model.Model;


public class Robot
{
    public int Id { get; init; }
    public (int row, int col) Pos { get; set; }
    public (int row, int col)? TargetPos { get; set; }
    public Direction Direction { get; private set; }
    public event EventHandler<int>? Finished;


    public Robot(int id, (int, int) pos, Direction direction)
    {
        Id = id;
        Pos = pos;
        TargetPos = null;
        Direction = direction;
    }

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

    private void OnFinished()
    {
        TargetPos = null;
        Finished?.Invoke(this, Id);
    }
}
