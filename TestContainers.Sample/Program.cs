using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using TestContainers.Sample.Database;
using TestContainers.Sample.EntityModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
});

builder.Services.AddDbContext<ProductDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDb"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/CreateProduct", async (CreateProductModel model, ProductDbContext db,ILogger<Program> logger) =>
    {
        var product = new ProductEntity()
        {
            ProductName = model.ProductName,
            ProductPrice = model.Price
        };

        await db.Products.AddAsync(product);

        logger.LogWarning($"Adding Product With ID: {product.Id}");
        
        
        await db.SaveChangesAsync();

        return Results.Created();
    }).WithOpenApi()
    .WithName("CreateProduct");

app.MapGet("/GetProducts", async ( ProductDbContext db) =>
    {
        var products = await db.Products.AsNoTracking()
            .Select(c => new GetProductModel(c.Id, c.ProductName, c.ProductPrice)).ToListAsync();

        return Results.Ok(products);
    }).WithOpenApi()
    .WithName("GetProducts");




app.Run();

public record CreateProductModel(string ProductName,decimal Price);

public record GetProductModel(int ProductId,string ProductName,decimal Price);

/// <summary>
/// Marker Interface To Declare The Project To Integration Tests
/// </summary>
public interface IApplicationTestMarker
{
    
}