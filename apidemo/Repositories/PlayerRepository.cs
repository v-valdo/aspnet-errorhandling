using apidemo.Data;
using apidemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class PlayerRepository : IPlayerRepository
{
    private readonly apidemoContext _context;
    private readonly IMemoryCache _cache;
    private const string CkAllPlayers = "AllPlayers";
    public PlayerRepository(apidemoContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }
    public async Task AddPlayerAsync(Player player)
    {
        await _context.AddAsync(player);
        await _context.SaveChangesAsync();

        _cache.Remove(CkAllPlayers);
    }

    public async Task DeletePlayerAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        _cache.Remove(CkAllPlayers);
    }
    public async Task<IEnumerable<Player>> GetAllPlayersAsync()
    {
        if (!_cache.TryGetValue(CkAllPlayers, out IEnumerable<Player>? players))
        {
            players = await _context.Players.ToListAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(CkAllPlayers, players, cacheEntryOptions);
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
