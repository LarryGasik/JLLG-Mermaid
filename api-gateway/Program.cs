using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("svc-a", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:SvcA"]!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("svc-b", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:SvcB"]!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("svc-c", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:SvcC"]!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", source = "api-gateway" }));

app.MapGet("/svc-a/items", async (IHttpClientFactory factory, HttpContext context) =>
{
    var client = factory.CreateClient("svc-a");
    var response = await client.GetAsync("/items", context.RequestAborted);
    return Results.Stream(await response.Content.ReadAsStreamAsync(context.RequestAborted), response.Content.Headers.ContentType?.ToString());
}).RequireAuthorization();

app.MapGet("/svc-b/items", async (IHttpClientFactory factory, HttpContext context) =>
{
    var client = factory.CreateClient("svc-b");
    var response = await client.GetAsync("/items", context.RequestAborted);
    return Results.Stream(await response.Content.ReadAsStreamAsync(context.RequestAborted), response.Content.Headers.ContentType?.ToString());
}).RequireAuthorization();

app.MapGet("/svc-c/items", async (IHttpClientFactory factory, HttpContext context) =>
{
    var client = factory.CreateClient("svc-c");
    var response = await client.GetAsync("/items", context.RequestAborted);
    return Results.Stream(await response.Content.ReadAsStreamAsync(context.RequestAborted), response.Content.Headers.ContentType?.ToString());
}).RequireAuthorization();

app.Run();
