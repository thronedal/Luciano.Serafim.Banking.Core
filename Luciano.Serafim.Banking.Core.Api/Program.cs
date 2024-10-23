using Luciano.Serafim.Banking.Core.Bootstrap;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAll(builder.Configuration);

builder.Logging.AddOpenTelemetry(otel =>
{
    otel.IncludeScopes = true;
    otel.IncludeFormattedMessage = true;
    otel.AddOtlpExporter();
});

var app = builder.Build();

app.UseApi();

app.Run();