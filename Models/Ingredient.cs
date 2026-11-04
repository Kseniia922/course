namespace RestaurantApp.Models;

public class Ingredient
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = "";
    public string IngredientCategory { get; set; } = "";
    public string? Measuring { get; set; }
}