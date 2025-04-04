using System.Data;
using Microsoft.Data.SqlClient;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void AddUser(string username, string email)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand("INSERT INTO Users (Username, Email) VALUES (@Username, @Email)", connection))
            {
                command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email });

                command.ExecuteNonQuery();
            }
        }
    }
}