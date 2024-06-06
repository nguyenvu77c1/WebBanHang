using Lab03.Data;
using Lab03.Models;
using Microsoft.EntityFrameworkCore;


public class EFConfigRepository : IConfigRepository
{
    private readonly ApplicationDbContext _context;

    public EFConfigRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Phương thức để lấy tất cả các đối tượng Config
    public async Task<IEnumerable<Config>> GetAllAsync()
    {
        return await _context.Configs.ToListAsync();
    }

    // Phương thức để lấy một đối tượng Config theo ID
    public async Task<Config> GetByIdAsync(int id)
    {
        return await _context.Configs.FindAsync(id);
    }

    // Phương thức để thêm một đối tượng Config mới
    public async Task AddAsync(Config config)
    {
        _context.Configs.Add(config);
        await _context.SaveChangesAsync();
    }

    // Phương thức để cập nhật một đối tượng Config
    public async Task UpdateAsync(Config config)
    {
        _context.Configs.Update(config);
        await _context.SaveChangesAsync();
    }

    // Phương thức để xóa một đối tượng Config theo ID
    public async Task DeleteAsync(int id)
    {
        var config = await _context.Configs.FindAsync(id);
        if (config != null)
        {
            _context.Configs.Remove(config);
            await _context.SaveChangesAsync();
        }
    }


}


