using apidemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace apidemo.Controllers;
[ApiController]
[Route("api/players")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<PlayerController> _logger;
    public PlayerController(IPlayerRepository playerRepository, ILogger<PlayerController> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
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
        try
        {
            var players = await _playerRepository.GetAllPlayersAsync();
            return Ok(players);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Woops, we have failed you...");
        }
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
}