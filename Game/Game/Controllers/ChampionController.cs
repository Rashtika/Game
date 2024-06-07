using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Game.Model;
using Game.Service;
using System.Numerics;
using Microsoft.Win32.SafeHandles;

namespace Game.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class ChampionController : ControllerBase
    {
        ChampionService championService = new ChampionService();


        [HttpGet(Name = "FindChampions")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var champions = await championService.GetAll();
                if (champions == null || !champions.Any())
                {
                    return NoContent();
                }
                return Ok(champions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateChampion(Guid id, [FromBody] Champion updatedChampion)
        {
            try
            {
                var champion = await championService.GetChampion(id);
                if (champion == null)
                {
                    return NotFound();
                }

                updatedChampion.Id = id;
                await championService.UpdateChampion(updatedChampion);
                return Ok("Champion updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost(Name = "InsertChampion")]
        public async Task<IActionResult> Insert([FromBody] Champion champion)
        {
            try
            {
                if (champion == null)
                {
                    return BadRequest("Champion is null.");
                }

                await championService.Insert(champion);
                return Ok("Champion added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChampion(Guid id)
        {
            try
            {
                var champion = await championService.GetChampion(id);
                if (champion == null)
                {
                    return NotFound();
                }
                return Ok(champion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChampion(Guid id)
        {
            try
            {
                var player = await championService.GetChampion(id);
                if (player == null)
                {
                    return NotFound();
                }

                await championService.DeleteChampion(id);
                return Ok("Champion deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
 }
