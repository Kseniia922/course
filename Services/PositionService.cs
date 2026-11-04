using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class PositionService
{
    public async Task<List<Position>> GetPositionsAsync()
    {
        var positions = new List<Position>();

        const string sql = """
            SELECT
                posada_id,
                posada_name
            FROM posada
            ORDER BY posada_name;
            """;

        using var connection = Database.GetConnection();

        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var position = new Position
            {
                PosadaId = Convert.ToInt32(reader["posada_id"]),
                PosadaName = reader.GetString(
                    reader.GetOrdinal("posada_name"))
            };

            positions.Add(position);
        }

        return positions;
    }
}