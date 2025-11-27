using Xunit;
using Microsoft.EntityFrameworkCore;
using Shifu.Models;
using Shifu.Services;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Shifu.Tests;
public class ResourceManagerTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddResourceAsync_AddsResource()
    {
        var context = GetInMemoryDb();
        var manager = new ResourceManager(context);

        var resource = new Resource { Title = "Test Resource", Url = "https://example.com" };
        await manager.AddResourceAsync(resource);

        var resources = await manager.GetAllResourcesAsync();
        Assert.Single(resources);
        Assert.Equal("Test Resource", resources.First().Title);
    }
}
