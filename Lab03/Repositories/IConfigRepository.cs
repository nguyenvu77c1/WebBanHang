using Lab03.Models;


public interface IConfigRepository
{
    Task<IEnumerable<Config>> GetAllAsync();
    Task<Config> GetByIdAsync(int id);
    Task AddAsync(Config Config);
    Task UpdateAsync(Config Config);
    Task DeleteAsync(int id);
}

