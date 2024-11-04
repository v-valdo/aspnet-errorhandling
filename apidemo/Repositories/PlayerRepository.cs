using apidemo;
using apidemo.Data;
using apidemo.Models;
using Microsoft.EntityFrameworkCore;

public class PlayerRepository : IPlayerRepository
{
    private readonly apidemoContext _context;
    public PlayerRepository(apidemoContext context)
    {
        _context = context;
    }
    public async Task AddPlayerAsync(Player player)
    {
        await _context.AddAsync(player);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePlayerAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Player>> GetAllPlayersAsync()
    {
        return await _context.Players.ToListAsync();
    }

    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        return await _context.Players.FindAsync(id);
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync();
    }

    public async Task<Player> GetPlayerByUsernameAsync(string username)
    {
        return await _context
        .Players
        .FirstOrDefaultAsync(p => p.Username == username);
    }
}
