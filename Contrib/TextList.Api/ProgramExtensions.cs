﻿using Dapr.Client;
using Dapr.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Polly;
using RecAll.Contrib.TextList.Api.Services;
using Serilog;

namespace RecAll.Contrib.TextList.Api;

public static class ProgramExtensions {
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;

    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder) {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder) {
        var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"];

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration).WriteTo.Console().WriteTo
            .Seq(seqServerUrl).Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
        builder.Services.AddSwaggerGen();


    public static void UseCustomSwagger(this WebApplication app) {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void AddCustomApplicationServices(
        this WebApplicationBuilder builder) {
        builder.Services.AddScoped<IIdentityService, MockIdentityService>();
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder) {
        builder.Services.AddDbContext<TextListContext>(options =>
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:TextListContext"]));
    }

    public static void ApplyDatabaseMigration(this WebApplication app) {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var context =
            scope.ServiceProvider.GetRequiredService<TextListContext>();

        retryPolicy.Execute(context.Database.Migrate);
    }

    private static Policy CreateRetryPolicy() {
        return Policy.Handle<Exception>().WaitAndRetryForever(
            sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
            onRetry: (exception, retry, _) => {
                Console.WriteLine(
                    "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                    exception.GetType().Name, exception.Message, retry);
            });
    }
}