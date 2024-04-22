namespace Warehouse_Simulation_Model.Model;


public class Target
{
    public int? Id { get; set; }
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
