namespace Warehouse_Simulation_Model.Model;


public class Robot
{
    public int Id { get; init; }
    public (int row, int col) Pos { get; private set; }
    public (int row, int col)? TargetPos { get; set; }
    public Direction Direction { get; private set; }
    public event EventHandler? Finished;


    public Robot(int id, (int, int) pos, Direction direction)
    {
        Id = id;
        Pos = pos;
        TargetPos = null;
        Direction = direction;
    }


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

    public void CheckPos()
    {
        if (Pos == TargetPos)
            OnFinished();
    }

    private void OnFinished()
    {
        TargetPos = null;
        Finished?.Invoke(this , EventArgs.Empty);
    }
}
