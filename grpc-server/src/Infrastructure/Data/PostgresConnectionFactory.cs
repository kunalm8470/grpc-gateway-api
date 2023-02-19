using Npgsql;

namespace Infrastructure.Data;

public class PostgresConnectionFactory
{
    public NpgsqlDataSource DataSource { get; }

    public PostgresConnectionFactory(string connectionString)
    {
        DataSource = NpgsqlDataSource.Create(connectionString);
    }
}
