using Shifu.Models;
using Microsoft.EntityFrameworkCore;

namespace Shifu.Services
{
    // created by Jonathan Ghattas  #991703952
    public class ResourceManager
    {
        private readonly AppDbContext _context;
        public ResourceManager(AppDbContext context) => _context = context;

        public async Task<List<Resource>> GetAllResourcesAsync()
            => await _context.Resources.ToListAsync();

        public async Task AddResourceAsync(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
        }
    }
}
