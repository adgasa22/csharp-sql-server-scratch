using Microsoft.Data.SqlClient;

public class Db : IDisposable
{
    public SqlConnection Connection { get; }

    public Db(string connectionString)
    {
        Connection = new SqlConnection(connectionString);
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}