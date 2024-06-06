using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Game.Model;
using Game.Service;

namespace Game.WebAPI.Controllers
{
        [ApiController]
        [Route("api/[controller]")]


        public class ChampionController : ControllerBase
        {

            string connectionString = "Host=localhost;Port=5433;Username=postgres;Password=Dakovo123;Database=ChampionDB;";

            private static List<Champion> Champions = new List<Champion>();

            private readonly ILogger<ChampionController> _logger;
            public ChampionController(ILogger<ChampionController> logger)
            {
                _logger = logger;
            }

            [HttpGet(Name = "FindChampions")]
            public async Task<IActionResult> GetAll()
            {
                List<Champion> champions = new List<Champion>();

                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM \"Champion\"";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                        {
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Champion champion = new Champion
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
                            }
                            connection.Close();
                        }
                    }
                    return Ok(champions);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occurred: " + ex.Message);
                }
            }

            [HttpPost(Name = "InsertChampion")]
            public async Task<IActionResult> Insert([FromBody] Champion champion)
            {
            if (champion == null)
            {
                return BadRequest();
            }

            using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string inventorySql = "INSERT INTO \"Inventory\" (\"DateCreated\") VALUES (@DateCreated) RETURNING \"Id\"";
                    Guid newInventoryId;
                    using (NpgsqlCommand inventoryCmd = new NpgsqlCommand(inventorySql, connection))
                    {
                        inventoryCmd.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                        newInventoryId = (Guid)inventoryCmd.ExecuteScalar();
                    }

                    string sql = "INSERT INTO \"Champion\" (\"Id\", \"Name\", \"InventoryId\", \"IsActive\", \"DateCreated\", \"CreatedByUserId\", \"UpdatedByUserId\") " +
                                     "VALUES (@Id, @Name, @InventoryId, @IsActive, @DateCreated, @CreatedByUserId, @UpdatedByUserId)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        champion.InventoryId = newInventoryId;
                        champion.DateCreated = DateTime.UtcNow;

                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@Name", champion.Name);
                        cmd.Parameters.AddWithValue("@InventoryId", champion.InventoryId);
                        cmd.Parameters.AddWithValue("@IsActive", champion.IsActive);
                        cmd.Parameters.AddWithValue("@DateCreated", champion.DateCreated);
                        cmd.Parameters.AddWithValue("@CreatedByUserId", champion.CreatedByUserId);
                        cmd.Parameters.AddWithValue("@UpdatedByUserId", champion.UpdatedByUserId);

                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return Ok("Product added successfully");
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetChampionById(Guid id)
            {
                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM \"Champion\" WHERE \"Id\" = @Id";
                        using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("Id", id);
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Champion champion = new Champion
                                    {
                                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        InventoryId = reader.GetGuid(reader.GetOrdinal("InventoryId")),
                                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                        DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                                        CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                                        UpdatedByUserId = reader.GetInt32(reader.GetOrdinal("UpdatedByUserId"))
                                    };


                                    return Ok(champion);
                                }
                            }
                        }
                        connection.Close();

                    }

                    return NotFound("Champion not found.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occurred: " + ex.Message);
                }
            }


            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteChampion(Guid id)
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();

                        string sql = "DELETE FROM \"Champion\" WHERE \"Id\" = @Id";
                        using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                        {
                            command.Parameters.AddWithValue("Id", id);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected == 0)
                                return NotFound("Champion not found.");

                            return Ok("Champion removed successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occurred: " + ex.Message);
                }
            }

            [HttpGet("test-connection", Name = "TestDatabaseConnection")]
            public IActionResult TestConnection()
            {
                try
                {
                    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                    connection.Open();
                    return Ok("Connection to the database is successful.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to the database.");
                    return StatusCode(500, "Failed to connect to the database.");
                }
            }
        }
    }
