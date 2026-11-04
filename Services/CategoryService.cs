using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class CategoryService
{
    public async Task<List<Category>> GetCategoriesAsync()
    {
        var categories = new List<Category>();

        const string sql = """
            SELECT
                category_id,
                category_name
            FROM category
            ORDER BY category_name;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var category = new Category
            {
                CategoryId = Convert.ToInt32(
                    reader["category_id"]),

                CategoryName = reader.GetString(
                    reader.GetOrdinal("category_name"))
            };

            categories.Add(category);
        }

        return categories;
    }
}