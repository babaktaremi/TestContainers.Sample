using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using TestContainers.Sample.Database;


namespace TestContainers.Sample.Test;

public class ApiApplicationFactory:WebApplicationFactory<IApplicationTestMarker>,IAsyncLifetime
{

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .Build();
        
        
        
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(c =>
        {
            
            var connection = new SqliteConnection("DataSource=:memory:");
            c.Remove(c.Single(a => typeof(DbContextOptions<ProductDbContext>) == a.ServiceType));
            c.AddDbContext<ProductDbContext>(options =>
            {
               options.UseSqlServer(_dbContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        using var scope = base.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}