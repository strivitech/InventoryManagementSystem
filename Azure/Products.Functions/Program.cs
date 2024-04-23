using System.Reflection;
using Azure.Identity;
using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Products.Functions.Database;
using Products.Functions.Repositories;
using Products.Functions.Services;
using Products.Functions.Validation;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((_, config) =>
    {
        var azureKeyVaultUrl = Environment.GetEnvironmentVariable("AzureKeyVaultUrl");

        if (!string.IsNullOrEmpty(azureKeyVaultUrl))
        {
            config.AddAzureKeyVault(
                new Uri(azureKeyVaultUrl),
                new DefaultAzureCredential());
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        services.AddDbContext<ProductsDbContext>(options =>
            options
                .UseNpgsql(
                    context.Configuration["ProductsPostgres"],
                    optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(ProductsDbContext).Assembly.FullName)));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.Decorate<IProductsRepository, CachedProductsRepository>();
        services.AddScoped<IRequestValidator, RequestValidator>();
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = context.Configuration["RedisCacheConfiguration"];
            options.InstanceName = context.Configuration["RedisCacheInstanceName"];
        });
    })
    .Build();

host.Run();
