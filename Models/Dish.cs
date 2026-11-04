namespace RestaurantApp.Models;

public class Dish
{
    public int DishId { get; set; }
    public string DishName { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string Weight { get; set; } = "";
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}