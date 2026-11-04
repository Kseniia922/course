using MySqlConnector;
using RestaurantApp.Data;

namespace RestaurantApp.Services;

public class DashboardService
{
    public async Task<int> GetDishesCountAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM dish;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> GetIngredientsCountAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM ingredient;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> GetEmployeesCountAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM employee;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> GetSalesCountAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM expense;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}