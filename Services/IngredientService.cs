using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class IngredientService
{
    public async Task<List<Ingredient>> GetIngredientsAsync()
    {
        var ingredients = new List<Ingredient>();

        const string sql = """
            SELECT
                ingredient_id,
                ingredient_name,
                ingredient_category,
                measuring
            FROM ingredient
            ORDER BY ingredient_name;
            """;

        using var connection = Database.GetConnection();

        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var ingredient = new Ingredient
            {
                IngredientId = Convert.ToInt32(
                    reader["ingredient_id"]),

                IngredientName = reader.GetString(
                    reader.GetOrdinal("ingredient_name")),

                IngredientCategory = reader.GetString(
                    reader.GetOrdinal("ingredient_category")),

                Measuring = reader.IsDBNull(
                    reader.GetOrdinal("measuring"))
                    ? null
                    : reader.GetString("measuring")
            };

            ingredients.Add(ingredient);
        }

        return ingredients;
    }


    public async Task AddIngredientAsync(Ingredient ingredient)
    {
        const string sql = """
            INSERT INTO ingredient
                (ingredient_name,
                ingredient_category,
                measuring)
            VALUES
                (@name,
                @category,
                @measuring);
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@name",
            ingredient.IngredientName);

        command.Parameters.AddWithValue(
            "@category",
            ingredient.IngredientCategory);

        command.Parameters.AddWithValue(
            "@measuring",
            ingredient.Measuring);

        await command.ExecuteNonQueryAsync();
    }


    public async Task UpdateIngredientAsync(Ingredient ingredient)
    {
        const string sql = """
            UPDATE ingredient
            SET
                ingredient_name = @name,
                ingredient_category = @category,
                measuring = @measuring
            WHERE ingredient_id = @id;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@name",
            ingredient.IngredientName);

        command.Parameters.AddWithValue(
            "@category",
            ingredient.IngredientCategory);

        command.Parameters.AddWithValue(
            "@measuring",
            ingredient.Measuring);

        command.Parameters.AddWithValue(
            "@id",
            ingredient.IngredientId);

        await command.ExecuteNonQueryAsync();
    }


    public async Task DeleteIngredientAsync(int ingredientId)
    {
        const string sql = """
            DELETE FROM ingredient
            WHERE ingredient_id = @id;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@id",
            ingredientId);

        await command.ExecuteNonQueryAsync();
    }
}