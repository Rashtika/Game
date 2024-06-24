using Game.Common;
using Game.Model;
using Game.Repository.Common;
using Npgsql;

namespace Game.Repository
{
    public class ChampionRepository : IGameRepository
    {
        private readonly string connectionString = "Host=localhost;Port=5433;Database=ChampionDB;Username=postgres;Password=Dakovo123;";

        public async Task DeleteChampion(Guid id)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            var cmdText = "DELETE FROM \"Champion\" WHERE \"Id\" = @Id;";
            await using var command = new NpgsqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateChampion(Champion champion)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Check if the InventoryId exists in the Inventory table
            var inventoryExistsCmdText = "SELECT COUNT(*) FROM \"Inventory\" WHERE \"Id\" = @InventoryId";
            await using (var inventoryExistsCmd = new NpgsqlCommand(inventoryExistsCmdText, connection))
            {
                inventoryExistsCmd.Parameters.AddWithValue("@InventoryId", champion.InventoryId);
                var inventoryCount = (long)await inventoryExistsCmd.ExecuteScalarAsync();
                if (inventoryCount == 0)
                {
                    // Inventory doesn't exist, create a new Inventory record
                    var newInventoryId = Guid.NewGuid();

                    string inventorySql = "INSERT INTO \"Inventory\" (\"Id\", \"IsActive\", \"DateCreated\", \"CreatedByUserId\", \"UpdatedByUserId\") VALUES (@Id, @IsActive, @DateCreated, @CreatedByUserId, @UpdatedByUserId)";
                    using (NpgsqlCommand inventoryCmd = new NpgsqlCommand(inventorySql, connection))
                    {
                        inventoryCmd.Parameters.AddWithValue("@Id", newInventoryId);
                        inventoryCmd.Parameters.AddWithValue("@IsActive", champion.Inventory.IsActive);
                        inventoryCmd.Parameters.AddWithValue("@DateCreated", champion.Inventory.DateCreated);
                        inventoryCmd.Parameters.AddWithValue("@CreatedByUserId", champion.Inventory.CreatedByUserId);
                        inventoryCmd.Parameters.AddWithValue("@UpdatedByUserId", champion.Inventory.UpdatedByUserId);
                        await inventoryCmd.ExecuteNonQueryAsync();
                    }

                    // Update Champion with the new InventoryId
                    champion.InventoryId = newInventoryId;
                }
            }

            // Update the Champion record
            var cmdText = "UPDATE \"Champion\" SET \"Name\" = @Name, \"InventoryId\" = @InventoryId, \"IsActive\" = @IsActive, \"DateCreated\" = @DateCreated, \"CreatedByUserId\" = @CreatedByUserId, \"UpdatedByUserId\" = @UpdatedByUserId WHERE \"Id\" = @Id;";
            await using (var command = new NpgsqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@Id", champion.Id);
                command.Parameters.AddWithValue("@Name", champion.Name);
                command.Parameters.AddWithValue("@InventoryId", champion.InventoryId);
                command.Parameters.AddWithValue("@IsActive", champion.IsActive);
                command.Parameters.AddWithValue("@DateCreated", champion.DateCreated);
                command.Parameters.AddWithValue("@CreatedByUserId", champion.CreatedByUserId);
                command.Parameters.AddWithValue("@UpdatedByUserId", champion.UpdatedByUserId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<Champion>> GetAll()
        {
            var champions = new List<Champion>();

            await using var connection = new NpgsqlConnection(connectionString);
            var cmdText = "SELECT * FROM \"Champion\";";

            await using var command = new NpgsqlCommand(cmdText, connection);
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var champion = new Champion
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    InventoryId = reader.GetGuid(reader.GetOrdinal("InventoryId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                    CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    UpdatedByUserId = reader.GetInt32(reader.GetOrdinal("UpdatedByUserId"))
                };
                champions.Add(champion);
            }

            return champions;
        }

        public async Task<Champion> GetChampion(Guid id)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            var cmdText = "SELECT * FROM \"Champion\" WHERE \"Id\" = @Id;";
            await using var command = new NpgsqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Champion
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    InventoryId = reader.GetGuid(reader.GetOrdinal("InventoryId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                    CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                    UpdatedByUserId = reader.GetInt32(reader.GetOrdinal("UpdatedByUserId"))

                };
            }
            return null;
        }

        public Task<IEnumerable<Champion>> GetFilteredChampion(ChampionFiltering filtering, ChampionSorting sorting, ChampionPaging paging)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(Champion champion)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Insert the Inventory first
            string inventorySql = "INSERT INTO \"Inventory\" (\"Id\", \"IsActive\", \"DateCreated\", \"CreatedByUserId\", \"UpdatedByUserId\") VALUES (@Id, @IsActive, @DateCreated, @CreatedByUserId, @UpdatedByUserId)";
            Guid newInventoryId = Guid.NewGuid();
            using (NpgsqlCommand inventoryCmd = new NpgsqlCommand(inventorySql, connection))
            {
                inventoryCmd.Parameters.AddWithValue("@Id", newInventoryId);
                inventoryCmd.Parameters.AddWithValue("@IsActive", champion.Inventory.IsActive);
                inventoryCmd.Parameters.AddWithValue("@DateCreated", champion.Inventory.DateCreated);
                inventoryCmd.Parameters.AddWithValue("@CreatedByUserId", champion.Inventory.CreatedByUserId);
                inventoryCmd.Parameters.AddWithValue("@UpdatedByUserId", champion.Inventory.UpdatedByUserId);
                await inventoryCmd.ExecuteNonQueryAsync();
            }

            // Insert the Champion with the InventoryId
            var sql = "INSERT INTO \"Champion\" (\"Id\", \"Name\", \"InventoryId\", \"IsActive\", \"DateCreated\", \"CreatedByUserId\", \"UpdatedByUserId\") VALUES (@Id, @Name, @InventoryId, @IsActive, @DateCreated, @CreatedByUserId, @UpdatedByUserId);";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", champion.Id);
            command.Parameters.AddWithValue("@Name", champion.Name);
            command.Parameters.AddWithValue("@InventoryId", newInventoryId);
            command.Parameters.AddWithValue("@IsActive", champion.IsActive);
            command.Parameters.AddWithValue("@DateCreated", champion.DateCreated);
            command.Parameters.AddWithValue("@CreatedByUserId", champion.CreatedByUserId);
            command.Parameters.AddWithValue("@UpdatedByUserId", champion.UpdatedByUserId);

            await command.ExecuteNonQueryAsync();
        }
    }
}
