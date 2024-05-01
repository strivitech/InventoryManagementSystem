using System.Reflection;
using Azure.Identity;
using FluentValidation;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Functions.Common;
using Ordering.Functions.Repositories;
using Ordering.Functions.Services;
using Ordering.Functions.Validation;
using Polly;
using Polly.Contrib.WaitAndRetry;

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

        services.AddSingleton(new CosmosClient(context.Configuration["Cosmos:ConnectionString"]!,
            new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                    { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
            }));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddHttpClient<IProductsService, ProductsService>(
                client => { client.BaseAddress = new Uri(context.Configuration["ProductsUrl"]!); })
            .AddHttpMessageHandler(b =>
                new AddQueryParameterAuthorizationCodeHandler(
                    context.Configuration["AuthorizationCodes:Products:GetProductOverviews"]!))
            .AddTransientHttpErrorPolicy(
                policyBuilder => policyBuilder.WaitAndRetryAsync(
                    Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(RequestPolly.MedianFirstRetryDelaySeconds),
                        RequestPolly.DefaultRetryCount)));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderingService, OrderingService>();
        services.AddScoped<IPreOrderingVerifier, PreOrderingVerifier>();
        services.AddScoped<IRequestValidator, RequestValidator>();
    })
    .Build();

host.Run();