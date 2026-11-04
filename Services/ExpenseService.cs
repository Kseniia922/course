using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class ExpenseService
{
    public async Task<List<Expense>> GetExpensesAsync()
    {
        var expenses = new List<Expense>();

        const string sql = """
            SELECT
                e.expense_id,
                e.dish_id,
                d.dish_name,
                e.date_expense,
                e.blanks,
                e.portions_sold
            FROM expense e
            JOIN dish d
                ON e.dish_id = d.dish_id
            ORDER BY e.date_expense DESC, e.expense_id DESC;
            """;

        using var connection = Database.GetConnection();

        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var expense = new Expense
            {
                ExpenseId = Convert.ToInt32(
                    reader["expense_id"]),

                DishId = Convert.ToInt32(
                    reader["dish_id"]),

                DishName = reader.GetString(
                    reader.GetOrdinal("dish_name")),

                DateExpense = reader.IsDBNull(
                    reader.GetOrdinal("date_expense"))
                    ? null
                    : reader.GetDateTime("date_expense"),

                Blanks = Convert.ToInt32(
                    reader["blanks"]),

                PortionsSold = Convert.ToInt32(
                    reader["portions_sold"])
            };

            expenses.Add(expense);
        }

        return expenses;
    }


    public async Task AddExpenseAsync(Expense expense)
    {
        const string sql = """
            INSERT INTO expense
                (dish_id,
                date_expense,
                blanks,
                portions_sold)
            VALUES
                (@dishId,
                @date,
                @blanks,
                @portionsSold);
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishId",
            expense.DishId);

        object dateValue = expense.DateExpense.HasValue
            ? expense.DateExpense.Value
            : DBNull.Value;

        command.Parameters.AddWithValue(
            "@date",
            dateValue);

        command.Parameters.AddWithValue(
            "@blanks",
            expense.Blanks);

        command.Parameters.AddWithValue(
            "@portionsSold",
            expense.PortionsSold);

        await command.ExecuteNonQueryAsync();
    }


    public async Task UpdateExpenseAsync(Expense expense)
    {
        const string sql = """
            UPDATE expense
            SET
                dish_id = @dishId,
                date_expense = @date,
                blanks = @blanks,
                portions_sold = @portionsSold
            WHERE expense_id = @expenseId;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@dishId",
            expense.DishId);

        command.Parameters.AddWithValue(
            "@date",
            expense.DateExpense.HasValue
                ? expense.DateExpense.Value
                : DBNull.Value);

        command.Parameters.AddWithValue(
            "@blanks",
            expense.Blanks);

        command.Parameters.AddWithValue(
            "@portionsSold",
            expense.PortionsSold);

        command.Parameters.AddWithValue(
            "@expenseId",
            expense.ExpenseId);

        await command.ExecuteNonQueryAsync();
    }


    public async Task DeleteExpenseAsync(int expenseId)
    {
        const string sql = """
            DELETE FROM expense
            WHERE expense_id = @expenseId;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@expenseId",
            expenseId);

        await command.ExecuteNonQueryAsync();
    }
}