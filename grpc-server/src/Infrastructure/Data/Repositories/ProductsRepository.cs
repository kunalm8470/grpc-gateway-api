using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.Requests;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Runtime.CompilerServices;

namespace Infrastructure.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly PostgresConnectionFactory _connectionFactory;

        public ProductsRepository(PostgresConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ProductModel> AddProductAsync(AddProductDto productDto, CancellationToken cancellationToken = default)
        {
            string sql = @"INSERT INTO
            inventory.products p (sku, name, manufactured_date, list_price)
            VALUES(@sku, @name, @manufactured_date, @list_price)
            RETURNING p.id as Id,
            p.sku as Sku,
            p.name as Name,
            p.manufactured_date as ManufacturedDate,
            p.list_price as ListPrice,
            p.created_at as CreatedAt,
            p.updated_at as UpdatedAt;";

            DynamicParameters parameters = new();

            parameters.Add("@sku", productDto.Sku, DbType.String, ParameterDirection.Input, productDto.Sku.Length);

            parameters.Add("@name", productDto.Name, DbType.String, ParameterDirection.Input, productDto.Name.Length);

            parameters.Add("@manufactured_date", productDto.ManufacturedDate, DbType.DateTime, ParameterDirection.Input);

            parameters.Add("@list_price", productDto.ListPrice, DbType.Double, ParameterDirection.Input);

            CommandDefinition definition = new(sql, parameters, cancellationToken: cancellationToken);

            using IDbConnection connection = _connectionFactory.DataSource.OpenConnection();

            return await connection.QuerySingleOrDefaultAsync<ProductModel>(definition);
        }

        public async Task<bool> DeleteProductAsync(string id, CancellationToken cancellationToken = default)
        {
            string sql = @"
            WITH deletedRows as (
                DELETE FROM inventory.products p
                WHERE id=@id
                RETURNING *
            )
            SELECT COUNT(*)
            FROM deletedRows;";

            DynamicParameters parameters = new();

            parameters.Add("@id", Guid.Parse(id), DbType.Guid, ParameterDirection.Input);

            CommandDefinition definition = new(sql, parameters, cancellationToken: cancellationToken);

            using IDbConnection connection = _connectionFactory.DataSource.OpenConnection();

            int numberOfRowsDeleted = await connection.ExecuteScalarAsync<int>(definition);

            return numberOfRowsDeleted > 1;
        }

        public async Task<ProductModel> GetProductsByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            string sql = @"SELECT p.id as Id,
            p.sku as Sku,
            p.name as Name,
            p.manufactured_date as ManufacturedDate,
            p.list_price as ListPrice,
            p.created_at as CreatedAt,
            p.updated_at as UpdatedAt
            FROM inventory.products p
            WHERE id=@id;";

            DynamicParameters parameters = new();

            parameters.Add("@id", Guid.Parse(id), DbType.Guid, ParameterDirection.Input);

            CommandDefinition definition = new(sql, parameters, cancellationToken: cancellationToken);

            using IDbConnection connection = _connectionFactory.DataSource.OpenConnection();

            return await connection.QuerySingleOrDefaultAsync<ProductModel>(definition);
        }

        public async IAsyncEnumerable<ProductModel> GetProductsPaginatedAsync(string searchAfter, int limit, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await using NpgsqlCommand command = _connectionFactory.DataSource.CreateCommand();
            
            string sql;

            if (string.IsNullOrEmpty(searchAfter))
            {
                sql = @"SELECT *
                FROM inventory.products p
                ORDER BY created_at
                LIMIT @limit;";
            }
            else
            {
                sql = @"SELECT *
                FROM inventory.products p
                WHERE created_at > @searchAfter
                ORDER BY created_at
                LIMIT @limit;";

                command.Parameters.AddWithValue("@searchAfter", NpgsqlDbType.Timestamp, DateTime.Parse(searchAfter));
            }

            command.Parameters.AddWithValue("@limit", NpgsqlDbType.Integer, limit);

            command.CommandText = sql;

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                ProductModel product = new()
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Sku = reader.GetString(reader.GetOrdinal("sku")),
                    ListPrice = reader.GetDouble(reader.GetOrdinal("list_price")),
                    ManufacturedDate = reader.GetDateTime(reader.GetOrdinal("manufactured_date")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetDateTime(reader.GetOrdinal("updated_at"))
                };

                yield return product;
            }
        }

        public async Task<ProductModel> UpdateProductAsync(string id, UpdateProductDto productDto, CancellationToken cancellationToken = default)
        {
            string sql = @"UPDATE inventory.products p
            SET sku=@sku,
            name=@name,
            manufactured_date=@manufactured_date,
            list_price = @list_price
            WHERE id=@id
            RETURNING p.id as Id,
            p.sku as Sku,
            p.name as Name,
            p.manufactured_date as ManufacturedDate,
            p.list_price as ListPrice,
            p.created_at as CreatedAt,
            p.updated_at as UpdatedAt;";

            DynamicParameters parameters = new();

            parameters.Add("@id", Guid.Parse(id), DbType.Guid, ParameterDirection.Input);

            parameters.Add("@sku", productDto.Sku, DbType.String, ParameterDirection.Input, productDto.Sku.Length);

            parameters.Add("@name", productDto.Name, DbType.String, ParameterDirection.Input, productDto.Name.Length);

            parameters.Add("@manufactured_date", productDto.ManufacturedDate, DbType.DateTime, ParameterDirection.Input);

            parameters.Add("@list_price", productDto.ListPrice, DbType.Double, ParameterDirection.Input);

            CommandDefinition definition = new(sql, parameters, cancellationToken: cancellationToken);

            using IDbConnection connection = _connectionFactory.DataSource.OpenConnection();

            return await connection.QuerySingleOrDefaultAsync<ProductModel>(definition);
        }
    }
}
