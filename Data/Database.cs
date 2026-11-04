using MySqlConnector;

namespace RestaurantApp.Data;

public static class Database
{
    private static readonly string connectionString =
        "Server=localhost;" +
        "Port=3306;" +
        "Database=restaurant;" +
        "User ID=root;" +
        "Password=Новыйпароль;";

    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }
}