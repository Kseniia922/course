namespace RestaurantApp.Models;

public class Recipe
{
    public int RecipeId { get; set; }

    public int? DishId { get; set; }

    public string DishName { get; set; } = "";

    public int? IngredientId { get; set; }

    public string IngredientName { get; set; } = "";

    public string AmountIngredient { get; set; } = "";

    public string Processing { get; set; } = "";
}