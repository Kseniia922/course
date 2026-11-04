using System;
using MySqlConnector;
using RestaurantApp.Data;
using System.Data;

namespace RestaurantApp.Services;

public class AnalyticsService
{
    // Популярні страви за категоріями
    public async Task<DataTable> GetPopularDishesAsync()
    {
        const string sql = """
            SELECT
                c.category_name AS 'Категорія',
                d.dish_name AS 'Страва',
                SUM(e.portions_sold) AS 'Продано порцій',
                RANK() OVER (
                    PARTITION BY c.category_name
                    ORDER BY SUM(e.portions_sold) DESC
                ) AS 'Рейтинг'
            FROM expense e
            JOIN dish d ON e.dish_id = d.dish_id
            JOIN category c ON d.category_id = c.category_id
            GROUP BY c.category_name, d.dish_id, d.dish_name
            ORDER BY c.category_name, 'Рейтинг';
            """;

        return await GetTableAsync(sql);
    }

    // Середня кількість проданих порцій на місяць
    public async Task<DataTable> GetAverageSalesAsync()
    {
        const string sql = """
            SELECT
                d.dish_name AS 'Страва',
                c.category_name AS 'Категорія',
                SUM(e.portions_sold) AS 'Усього порцій',
                ROUND(
                    SUM(e.portions_sold) /
                    (
                        TIMESTAMPDIFF(
                            MONTH,
                            (SELECT MIN(date_expense) FROM expense),
                            (SELECT MAX(date_expense) FROM expense)
                        ) + 1
                    ),
                    2
                ) AS 'У середньому за місяць'
            FROM expense e
            JOIN dish d ON e.dish_id = d.dish_id
            JOIN category c ON d.category_id = c.category_id
            GROUP BY d.dish_id, d.dish_name, c.category_name;
            """;

        return await GetTableAsync(sql);
    }

    // Продажі за місяцями
    public async Task<DataTable> GetMonthlySalesAsync()
    {
        const string sql = """
            SELECT
                d.dish_name AS 'Назва страви',
                c.category_name AS 'Категорія',
                DATE_FORMAT(e.date_expense, '%m %Y') AS 'Місяць',
                SUM(e.portions_sold) AS 'Кількість порцій',
                SUM(e.portions_sold * d.price) AS 'Загальна сума'
            FROM expense e
            JOIN dish d ON e.dish_id = d.dish_id
            JOIN category c ON d.category_id = c.category_id
            GROUP BY
                d.dish_id,
                d.dish_name,
                c.category_id,
                c.category_name,
                YEAR(e.date_expense),
                MONTH(e.date_expense),
                DATE_FORMAT(e.date_expense, '%m %Y')
            ORDER BY
                YEAR(e.date_expense),
                MONTH(e.date_expense),
                d.dish_name;
            """;

        return await GetTableAsync(sql);
    }


    
    // Звіт про продажі за обрану дату
    public async Task<DataTable> GetDailySalesAsync(DateTime selectedDate)
    {
        const string sql = """
            SELECT
                d.dish_name AS 'Назва страви',
                c.category_name AS 'Категорія',
                SUM(e.portions_sold) AS 'Продано порцій',
                d.price AS 'Ціна за одиницю',
                SUM(e.portions_sold * d.price) AS 'Загальна сума'
            FROM expense e
            JOIN dish d ON e.dish_id = d.dish_id
            JOIN category c ON d.category_id = c.category_id
            WHERE e.date_expense = @selectedDate
            GROUP BY
                d.dish_id,
                d.dish_name,
                c.category_name,
                d.price
            ORDER BY d.dish_name;
            """;

        var table = new DataTable();

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@selectedDate",
            selectedDate.Date);

        using var reader = await command.ExecuteReaderAsync();

        table.Load(reader);

        return table;
    }


    // Отримання результату SQL-запиту у вигляді таблиці
    private async Task<DataTable> GetTableAsync(string sql)
    {
        var table = new DataTable();

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        table.Load(reader);

        return table;
    }

    
    // Підвищення цін на страви обраної категорії
    public async Task RaisePricesAsync(string categoryName, decimal percent)
    {
        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "CALL Raise_Price(@categoryName, @percent);",
            connection);

        command.Parameters.AddWithValue("@categoryName", categoryName);
        command.Parameters.AddWithValue("@percent", percent);

        await command.ExecuteNonQueryAsync();
    }
}