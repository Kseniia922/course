namespace RestaurantApp.Models;

public class Expense
{
    public int ExpenseId { get; set; }

    public int DishId { get; set; }

    public string DishName { get; set; } = "";

    public DateTime? DateExpense { get; set; }

    public int Blanks { get; set; }

    public int PortionsSold { get; set; }
}