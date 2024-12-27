using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;
using Luciano.Serafim.Banking.Core.Bootstrap.Filters;
using Luciano.Serafim.Banking.Core.MediatR;
using Luciano.Serafim.Banking.Core.Abstractions.Transactions;
using Luciano.Serafim.Banking.Core.Models;
using Luciano.Serafim.Banking.Core.Infrastructure.MongoDb;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using StackExchange.Redis;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Luciano.Serafim.Banking.Core.Bootstrap;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services, params Assembly[] extraAssemblies)
    {
        Assembly[] assemblies = [Assembly.Load("Luciano.Serafim.Banking.Core.UseCases")];

        if (extraAssemblies.Length > 0)
        {
            assemblies = assemblies.Concat(extraAssemblies).ToArray();
        }

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddOpenBehavior(typeof(LockingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AcidBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CachingInvalidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CachingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        return services;
    }

    public static IServiceCollection AddDistributedCacheConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("RedisCache");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            //use memcache for testing
            services.AddDistributedMemoryCache();

        }
        else
        {
            var opt = ConfigurationOptions.Parse(connectionString);
            var conn = ConnectionMultiplexer.Connect(opt);

            services.AddStackExchangeRedisCache(options => options.ConfigurationOptions = opt);

        }
        return services;
    }

    public static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            //register filters globally
            options.Filters.Add<ExceptionFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection AddHttpLoggingConfiguration(this IServiceCollection services)
    {
        //configure Http Logging (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging)
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            //logging.RequestHeaders.Add("sec-ch-ua");
            //logging.ResponseHeaders.Add("MyResponseHeader");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        }); ;

        return services;
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen(c =>
        {
            // Set the comments path for the Swagger JSON and UI.
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml"));
            //search for project documentation files
            var docFiles = Directory.GetFiles(AppContext.BaseDirectory, "Luciano.Serafim.*.xml");
            foreach (var file in docFiles)
            {
                c.IncludeXmlComments(file);
            }
        });

        return services;
    }

    public static IServiceCollection AddResponse(this IServiceCollection services)
    {
        services.AddScoped(typeof(Response<>), typeof(Response<>));

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var accountDatabase = configuration.GetConnectionString("AccountDatabase");

        if (accountDatabase is null)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IAccountRepository, Infrastructure.AccountRepository>();
            services.AddSingleton<IEventRepository, Infrastructure.EventRepository>();
        }
        else
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = MongoClientSettings.FromConnectionString(accountDatabase);
                settings.LoggingSettings = new LoggingSettings(sp.GetService<ILoggerFactory>());
                return new MongoClient(settings);
            });

            services.AddScoped<IUnitOfWork, Infrastructure.MongoDb.UnitOfWork>();
            services.AddScoped<IAccountRepository, Infrastructure.MongoDb.AccountRepository>();
            services.AddScoped<IEventRepository, Infrastructure.MongoDb.EventRepository>();

            Mapping.MapEntities();
        }

        return services;
    }

    public static IServiceCollection AddDistributedLock(this IServiceCollection services, ConfigurationManager configuration)
    {
        //connection string format: https://stackexchange.github.io/StackExchange.Redis/Configuration.html
        var connectionString = configuration.GetConnectionString("DistributedLock");
        var options = ConfigurationOptions.Parse(connectionString);
        var conn = ConnectionMultiplexer.Connect(options);

        services.AddSingleton<IDistributedLockProvider>(_ => new RedisDistributedSynchronizationProvider(conn.GetDatabase()));

        return services;
    }

    public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        Action<ResourceBuilder> appResourceBuilder = resource => resource
            .AddContainerDetector()
            .AddHostDetector();

        var connectionString = configuration.GetConnectionString("RedisCache");
        var options = ConfigurationOptions.Parse(connectionString);
        var conn = ConnectionMultiplexer.Connect(options);            

        services.AddOpenTelemetry()
        .ConfigureResource(appResourceBuilder)
        .WithTracing(tracerBuilder => tracerBuilder
            .AddRedisInstrumentation(conn, options => options.SetVerboseDatabaseStatements = true)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter())
        .WithMetrics(meterBuilder => meterBuilder
            .AddProcessInstrumentation()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter());

        return services;
    }

    public static IServiceCollection AddAll(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddMediatRConfiguration()
            .AddLogging()
            .AddHttpLoggingConfiguration()
            .AddDistributedCacheConfiguration(configuration)
            .AddControllersConfiguration()
            .AddSwaggerConfiguration()
            .AddResponse()
            .AddDistributedLock(configuration)
            .AddOpenTelemetryConfiguration(configuration)
            .AddServices(configuration);

        return services;
    }
}
