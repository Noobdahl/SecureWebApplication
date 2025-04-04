using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class AuthorizationService
{
    private readonly string _connectionString;

    public AuthorizationService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("DefaultConnection", "Connection string cannot be null.");
    }

    public bool IsUserInRole(string username, string role)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT COUNT(1) FROM UserRoles WHERE Username = @Username AND Role = @Role", connection))
            {
                command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
                command.Parameters.Add(new SqlParameter("@Role", SqlDbType.NVarChar) { Value = role });

                return (int)command.ExecuteScalar() == 1;
            }
        }
    }
}