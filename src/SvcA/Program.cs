using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(builder.Configuration["Mongo:ConnectionString"]));
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(builder.Configuration["Mongo:Database"]);
    return database.GetCollection<Item>(builder.Configuration["Mongo:Collection"]);
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", source = "svc-a" }));

app.MapGet("/items", async (IMongoCollection<Item> collection) =>
{
    var items = await collection.Find(FilterDefinition<Item>.Empty).ToListAsync();
    return Results.Ok(items);
}).RequireAuthorization();

app.MapPost("/items", async (IMongoCollection<Item> collection, Item item) =>
{
    await collection.InsertOneAsync(item);
    return Results.Created($"/items/{item.Id}", item);
}).RequireAuthorization();

app.Run();

internal class Item
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
