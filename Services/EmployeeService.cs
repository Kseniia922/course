using MySqlConnector;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Services;

public class EmployeeService
{
    public async Task<List<Employee>> GetEmployeesAsync()
    {
        var employees = new List<Employee>();

        const string sql = """
            SELECT
                e.employee_id,
                e.employee_fullname,
                e.employee_phone,
                e.birth,
                e.sex,
                e.posada_id,
                e.direction,
                p.posada_name
            FROM employee e
            JOIN posada p
                ON e.posada_id = p.posada_id
            ORDER BY e.posada_id;
            """;

        using var connection = Database.GetConnection();

        await connection.OpenAsync();

        using var command = new MySqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var employee = new Employee
            {
                EmployeeId = reader.GetInt32("employee_id"),
                EmployeeFullname = reader.GetString("employee_fullname"),
                EmployeePhone = reader.GetString("employee_phone"),
                Birth = reader.IsDBNull(reader.GetOrdinal("birth"))
                    ? null
                    : reader.GetDateTime("birth"),
                Sex = reader.IsDBNull(reader.GetOrdinal("sex"))
                    ? null
                    : reader.GetString("sex"),
                PosadaId = reader.GetInt32("posada_id"),
                Direction = reader.IsDBNull(reader.GetOrdinal("direction"))
                    ? null
                    : reader.GetString("direction"),
                PositionName = reader.GetString(
                    reader.GetOrdinal("posada_name"))
            };

            employees.Add(employee);
        }

        return employees;
    }




    public async Task AddEmployeeAsync(Employee employee)
{
    const string sql = """
        INSERT INTO employee
            (employee_fullname,
             employee_phone,
             birth,
             sex,
             posada_id,
             direction)
        VALUES
            (@fullname,
            @phone,
            @birth,
            @sex,
            @posadaId,
            @direction);
        """;

    using var connection = Database.GetConnection();

    await connection.OpenAsync();

    using var command = new MySqlCommand(sql, connection);

    command.Parameters.AddWithValue(
        "@fullname",
        employee.EmployeeFullname);

    command.Parameters.AddWithValue(
        "@phone",
        employee.EmployeePhone);

    command.Parameters.AddWithValue(
        "@birth",
        employee.Birth);

    command.Parameters.AddWithValue(
        "@sex",
        employee.Sex);

    command.Parameters.AddWithValue(
        "@posadaId",
        employee.PosadaId);

    command.Parameters.AddWithValue(
        "@direction",
        employee.Direction);

    await command.ExecuteNonQueryAsync();
}


    public async Task UpdateEmployeeAsync(Employee employee)
{
    const string sql = """
        UPDATE employee
        SET
            employee_fullname = @fullname,
            employee_phone = @phone,
            birth = @birth,
            sex = @sex,
            posada_id = @posadaId,
            direction = @direction
        WHERE employee_id = @employeeId;
        """;

    using var connection = Database.GetConnection();

    await connection.OpenAsync();

    using var command = new MySqlCommand(sql, connection);

    command.Parameters.AddWithValue(
        "@fullname",
        employee.EmployeeFullname);

    command.Parameters.AddWithValue(
        "@phone",
        employee.EmployeePhone);

    command.Parameters.AddWithValue(
        "@birth",
        employee.Birth);

    command.Parameters.AddWithValue(
        "@sex",
        employee.Sex);

    command.Parameters.AddWithValue(
        "@posadaId",
        employee.PosadaId);

    command.Parameters.AddWithValue(
        "@direction",
        employee.Direction);

    command.Parameters.AddWithValue(
        "@employeeId",
        employee.EmployeeId);

    await command.ExecuteNonQueryAsync();
}


    public async Task DeleteEmployeeAsync(int employeeId)
{
    const string sql = """
        DELETE FROM employee
        WHERE employee_id = @employeeId;
        """;

    using var connection = Database.GetConnection();

    await connection.OpenAsync();

    using var command = new MySqlCommand(sql, connection);

    command.Parameters.AddWithValue(
        "@employeeId",
        employeeId);

    await command.ExecuteNonQueryAsync();
}
}