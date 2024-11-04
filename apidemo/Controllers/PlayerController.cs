using apidemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace apidemo.Controllers;
[ApiController]
[Route("api/players")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerRepository _playerRepository;
    public PlayerController(IPlayerRepository playerRepository, IMemoryCache memoryCache)
    {
        _playerRepository = playerRepository;
    }

    // throw error demonstration
    [HttpGet("throw")]
    public IActionResult ExceptionDemo()
    {
        throw new Exception("Test exception");
    }

    // GET api/players/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var players = await _playerRepository.GetAllPlayersAsync();
        if (players == null || !players.Any())
        {
            return NotFound("No players found");
        }
        return Ok(players);
    }

    // GET api/players/id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPlayerById(int id)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(id);
        if (player != null)
        {
            return Ok(player);
        }
        // If ID not found
        return NotFound($"No player with id {id} found");
    }

    // GET api/players/username
    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetPlayerByUsername(string username)
    {
        var user = await _playerRepository.GetPlayerByUsernameAsync(username);

        if (user != null)
        {
            return Ok(user);
        }
        return NotFound(new { message = $"No player with username {username} could be found!" });
    }

    // POST api/players
    [HttpPost]
    public async Task<IActionResult> AddPlayer(Player player)
    {
        // check model binding
        if (!ModelState.IsValid)
        {
            return BadRequest("Input doesn't match required fields");
        }
        // check for duplicate user
        var dupCheck = await _playerRepository.GetPlayerByUsernameAsync(player.Username);
        if (dupCheck != null)
        {
            return Conflict(new { message = "A player with this username already exists" });
        }

        await _playerRepository.AddPlayerAsync(player);
        return CreatedAtAction(nameof(GetPlayerById), new { id = player.Id }, player);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        bool successfulDeletion = await _playerRepository.DeletePlayerAsync(id);
        if (successfulDeletion)
        {
            return NoContent();
        }
        return NotFound($"Player with ID {id} was not found");
    }

}