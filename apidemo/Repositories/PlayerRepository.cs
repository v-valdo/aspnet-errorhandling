using apidemo.Data;
using apidemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class PlayerRepository : IPlayerRepository
{
    private readonly apidemoContext _context;
    private readonly IMemoryCache _cache;
    private ILogger<PlayerRepository> _logger;
    private const string CkAllPlayers = "AllPlayers";

    public PlayerRepository(apidemoContext context, IMemoryCache cache, ILogger<PlayerRepository> logger)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task AddPlayerAsync(Player player)
    {
        await _context.AddAsync(player);
        await _context.SaveChangesAsync();
        _cache.Remove(CkAllPlayers);
        _logger.LogInformation($"Added new player {player.Username}");
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted player {player.Username}");

            _cache.Remove(CkAllPlayers);

            return true;
        }
        return false;
    }
    public async Task<IEnumerable<Player>> GetAllPlayersAsync()
    {
        if (!_cache.TryGetValue(CkAllPlayers, out IEnumerable<Player>? players))
        {
            _logger.LogInformation("Players not found in cache. Fetching from database...");

            players = await _context.Players.ToListAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(CkAllPlayers, players, cacheEntryOptions);

            _logger.LogInformation($"Fetched {players.Count()} players from database and cached them.");
        }
        else
        {
            _logger.LogInformation("Retrieved players from cache.");
        }
        return players;
    }

    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        return await _context.Players.FindAsync(id);
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync();
        _cache.Remove(CkAllPlayers);
    }

    public async Task<Player> GetPlayerByUsernameAsync(string username)
    {
        return await _context
        .Players
        .FirstOrDefaultAsync(p => p.Username == username);
    }
}
