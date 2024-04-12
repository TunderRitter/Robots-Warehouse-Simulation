namespace Warehouse_Simulation_Model.Model;


public class Target
{
    private int? _id;
    public int? Id
    {
        get => _id;
        set => _id ??= value;
    }
    public (int row, int col) Pos { get; init; }
    public bool Active { get; set; }
    public int InitId { get; init; }


    public Target((int, int) pos, int initId)
    {
        Pos = pos;
        Active = false;
        InitId = initId;
    }
}
