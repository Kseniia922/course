using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class DishService
{
    public async Task<List<Dish>> GetDishesAsync()
    {
        var dishes = new List<Dish>();

        const string sql = """
            SELECT
                d.dish_id,
                d.dish_name,
                d.category_id,
                c.category_name,
                d.weight,
                d.price,
                d.description
            FROM dish d
            JOIN category c
                ON d.category_id = c.category_id
            ORDER BY d.category_id;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var dish = new Dish
            {
                DishId = Convert.ToInt32(reader["dish_id"]),

                DishName = reader.GetString(
                    reader.GetOrdinal("dish_name")),

                CategoryId = Convert.ToInt32(
                    reader["category_id"]),

                CategoryName = reader.GetString(
                    reader.GetOrdinal("category_name")),

                Weight = reader.GetString(
                    reader.GetOrdinal("weight")),

                Price = reader.IsDBNull(
                    reader.GetOrdinal("price"))
                    ? null
                    : reader.GetDecimal("price"),

                Description = reader.IsDBNull(
                    reader.GetOrdinal("description"))
                    ? null
                    : reader.GetString("description")
            };

            dishes.Add(dish);
        }

        return dishes;
    }


    public async Task AddDishAsync(Dish dish)
    {
        const string sql = """
            INSERT INTO dish
                (dish_name,
                category_id,
                weight,
                price,
                description)
            VALUES
                (@dishName,
                @categoryId,
                @weight,
                @price,
                @description);
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishName",
            dish.DishName);

        command.Parameters.AddWithValue(
            "@categoryId",
            dish.CategoryId);

        command.Parameters.AddWithValue(
            "@weight",
            dish.Weight);

        command.Parameters.AddWithValue(
            "@price",
            dish.Price);

        command.Parameters.AddWithValue(
            "@description",
            dish.Description);

        await command.ExecuteNonQueryAsync();
    }


    public async Task UpdateDishAsync(Dish dish)
    {
        const string sql = """
            UPDATE dish
            SET
                dish_name = @dishName,
                category_id = @categoryId,
                weight = @weight,
                price = @price,
                description = @description
            WHERE dish_id = @dishId;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishName",
            dish.DishName);

        command.Parameters.AddWithValue(
            "@categoryId",
            dish.CategoryId);

        command.Parameters.AddWithValue(
            "@weight",
            dish.Weight);

        command.Parameters.AddWithValue(
            "@price",
            dish.Price);

        command.Parameters.AddWithValue(
            "@description",
            dish.Description);

        command.Parameters.AddWithValue(
            "@dishId",
            dish.DishId);

        await command.ExecuteNonQueryAsync();
    }


    public async Task DeleteDishAsync(int dishId)
    {
        const string sql = """
            DELETE FROM dish
            WHERE dish_id = @dishId;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishId",
            dishId);

        await command.ExecuteNonQueryAsync();
    }
}