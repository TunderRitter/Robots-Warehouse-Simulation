namespace Warehouse_Simulation_Model.Model;


public static class TaskAssignerFactory
{
    public static ITaskAssigner Create(string type)
    {
        return type switch
        {
            "roundrobin" => new RoundRobin(),
            _ => throw new InvalidDataException("Not supported task assigner"),
        };
    }
}
