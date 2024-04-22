using Azure.Identity;
using Inventory.PostgresMigrationsApp.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUrl = builder.Configuration["AzureKeyVault:VaultUrl"]!;

builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options
        .UseNpgsql(
            builder.Configuration["InventoryPostgres"],
            optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

try
{
    app.ApplyMigrations();
    app.Run();
}   
catch (Exception ex)
{
    throw;
};