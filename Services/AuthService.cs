
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class AuthService
{
    private const int Iterations = 100000;

    // Перевірка логіна та пароля
    public async Task<AppUser?> LoginAsync(string username, string password)
    {
        const string sql = """
            SELECT user_id, username, password_hash, role, is_active
            FROM app_user
            WHERE username = @username
            LIMIT 1;
            """;

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        string passwordHash = reader.GetString("password_hash");
        bool isActive = reader.GetBoolean("is_active");

        if (!isActive || !VerifyPassword(password, passwordHash))
            return null;

        return new AppUser
        {
            UserId = Convert.ToInt32(reader["user_id"]),
            Username = reader.GetString("username"),
            Role = reader.GetString("role"),
            IsActive = isActive
        };
    }

    // Створення хеша пароля
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            32
        );

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    // Перевірка введеного пароля
    private static bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            string[] parts = storedHash.Split('.');

            if (parts.Length != 3)
                return false;

            int iterations = int.Parse(parts[0]);

            if (iterations < 100000 || iterations > 1000000)
                return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] originalHash = Convert.FromBase64String(parts[2]);

            byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                originalHash.Length
            );

            return CryptographicOperations.FixedTimeEquals(
                enteredHash,
                originalHash
            );
        }
        catch
        {
            return false;
        }
    }

    // Створення трьох облікових записів
    public async Task CreateDefaultUsersAsync()
    {
        var users = new[]
        {
            (Username: "Director", Password: "directorrest01", Role: "Director"),
            (Username: "Admin", Password: "adminrest02", Role: "Admin"),
            (Username: "Chef", Password: "chefrest03", Role: "Chef")
        };

        using var connection = Database.GetConnection();
        await connection.OpenAsync();

        foreach (var user in users)
        {
            const string checkSql = """
                SELECT COUNT(*)
                FROM app_user
                WHERE username = @username;
                """;

            using var checkCommand = new MySqlCommand(checkSql, connection);
            checkCommand.Parameters.AddWithValue("@username", user.Username);

            int count = Convert.ToInt32(
                await checkCommand.ExecuteScalarAsync());

            if (count > 0)
                continue;

            const string insertSql = """
                INSERT INTO app_user (username, password_hash, role)
                VALUES (@username, @passwordHash, @role);
                """;

            using var insertCommand = new MySqlCommand(insertSql, connection);

            insertCommand.Parameters.AddWithValue("@username", user.Username);
            insertCommand.Parameters.AddWithValue(
                "@passwordHash", HashPassword(user.Password));
            insertCommand.Parameters.AddWithValue("@role", user.Role);

            await insertCommand.ExecuteNonQueryAsync();
        }
    }
}
