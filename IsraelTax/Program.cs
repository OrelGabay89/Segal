using IsraeliTaxTokenFetcher.Services;
using System.IO;
using IsraelTax.Interfaces;
using IsraelTax.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5001); // Set Kestrel to listen on port 5001
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clientcredentials.db"));
builder.Services.AddHttpClient<TokenService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddControllers();

// Register Swagger generator and configure Swagger options
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Registers Swagger generator

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // This makes Swagger UI available at <root>/swagger
    });
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // This line is crucial
});

app.Run();
