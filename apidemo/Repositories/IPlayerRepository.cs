using apidemo;
using apidemo.Models;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllPlayersAsync();
    Task<Player?> GetPlayerByIdAsync(int id);
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerAsync(Player player);
    Task DeletePlayerAsync(int id);

    Task<Player> GetPlayerByUsernameAsync(string username);
}