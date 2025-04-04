using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class AuthService
{
    private readonly string _connectionString;

    public AuthService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("DefaultConnection", "Connection string cannot be null.");
    }

    public bool AuthenticateUser(string username, string password)
    {
        // Hash the user-provided password
        var hashedPassword = HashPassword(password);

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash", connection))
            {
                command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
                command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarBinary) { Value = hashedPassword });

                return (int)command.ExecuteScalar() == 1;
            }
        }
    }

    public List<string> GetUserRoles(string username)
    {
        var roles = new List<string>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT Role FROM UserRoles WHERE Username = @Username", connection))
            {
                command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
                    }
                }
            }
        }

        return roles;
    }

    private byte[] HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}