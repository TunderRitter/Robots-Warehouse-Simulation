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


    public void Turn(string dir)
    {

    }

    private void OnFinished() => Finished?.Invoke(this, Id);
}
