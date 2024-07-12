using System.Net;
using System.Net.Http.Json;

namespace TestContainers.Sample.Test;

public class IntegrationTests:IClassFixture<ApiApplicationFactory>
{
    private readonly HttpClient _client;
    public IntegrationTests(ApiApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Creating_Product_Should_Return_Created_StatusCode()
    {
        var product = new CreateProductModel("Test",3.99M);

        var response = await _client.PostAsJsonAsync("CreateProduct", product);
        
        Assert.Equal(HttpStatusCode.Created,response.StatusCode);
        
        
    }
}