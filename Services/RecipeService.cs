using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class RecipeService
{
    public async Task<List<Recipe>> GetRecipesAsync()
    {
        var recipes = new List<Recipe>();

        const string sql = """
            SELECT
                r.recipe_id,
                r.dish_id,
                d.dish_name,
                r.ingredient_id,
                i.ingredient_name,
                r.amount_ingredient,
                r.processing
            FROM recipe r
            LEFT JOIN dish d
                ON r.dish_id = d.dish_id
            LEFT JOIN ingredient i
                ON r.ingredient_id = i.ingredient_id
            ORDER BY d.dish_name, i.ingredient_name;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var recipe = new Recipe
            {
                RecipeId = Convert.ToInt32(
                    reader["recipe_id"]),

                DishId = reader.IsDBNull(
                    reader.GetOrdinal("dish_id"))
                    ? null
                    : Convert.ToInt32(reader["dish_id"]),

                DishName = reader.IsDBNull(
                    reader.GetOrdinal("dish_name"))
                    ? ""
                    : reader.GetString("dish_name"),

                IngredientId = reader.IsDBNull(
                    reader.GetOrdinal("ingredient_id"))
                    ? null
                    : Convert.ToInt32(
                        reader["ingredient_id"]),

                IngredientName = reader.IsDBNull(
                    reader.GetOrdinal("ingredient_name"))
                    ? ""
                    : reader.GetString("ingredient_name"),

                AmountIngredient = reader.GetString(
                    reader.GetOrdinal("amount_ingredient")),

                Processing = reader.GetString(
                    reader.GetOrdinal("processing"))
            };

            recipes.Add(recipe);
        }

        return recipes;
    }


    public async Task AddRecipeAsync(Recipe recipe)
    {
        const string sql = """
            INSERT INTO recipe
                (dish_id,
                ingredient_id,
                amount_ingredient,
                processing)
            VALUES
                (@dishId,
                @ingredientId,
                @amount,
                @processing);
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishId",
            recipe.DishId);

        command.Parameters.AddWithValue(
            "@ingredientId",
            recipe.IngredientId);

        command.Parameters.AddWithValue(
            "@amount",
            recipe.AmountIngredient);

        command.Parameters.AddWithValue(
            "@processing",
            recipe.Processing);

        await command.ExecuteNonQueryAsync();
    }


    public async Task UpdateRecipeAsync(Recipe recipe)
    {
        const string sql = """
            UPDATE recipe
            SET
                dish_id = @dishId,
                ingredient_id = @ingredientId,
                amount_ingredient = @amount,
                processing = @processing
            WHERE recipe_id = @recipeId;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishId",
            recipe.DishId);

        command.Parameters.AddWithValue(
            "@ingredientId",
            recipe.IngredientId);

        command.Parameters.AddWithValue(
            "@amount",
            recipe.AmountIngredient);

        command.Parameters.AddWithValue(
            "@processing",
            recipe.Processing);

        command.Parameters.AddWithValue(
            "@recipeId",
            recipe.RecipeId);

        await command.ExecuteNonQueryAsync();
    }



    public async Task DeleteRecipeAsync(int recipeId)
    {
        const string sql = """
            DELETE FROM recipe
            WHERE recipe_id = @recipeId;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@recipeId",
            recipeId);

        await command.ExecuteNonQueryAsync();
    }
}