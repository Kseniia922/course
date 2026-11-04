namespace RestaurantApp.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string EmployeeFullname { get; set; } = "";
    public string EmployeePhone { get; set; } = "";
    public DateTime? Birth { get; set; }
    public string? Sex { get; set; }
    public int PosadaId { get; set; }
    public string? Direction { get; set; }
    public string PositionName { get; set; } = "";
}

