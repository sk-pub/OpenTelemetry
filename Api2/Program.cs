using Api2;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Telemetry
    var serviceName = "Api2";
    var serviceVersion = "1.0.0";
    var serviceInstanceId = Environment.MachineName;

    var telemetryBuilder = ResourceBuilder.CreateDefault()
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion, serviceInstanceId: serviceInstanceId);

    builder.Services.AddOpenTelemetryTracing(b => b
        // No console exporter - console logs are done by Serilog
        //.AddConsoleExporter()
        .AddJaegerExporter(b => b.Endpoint = new Uri("http://localhost:6831"))
        .AddSource(serviceName)
        .SetResourceBuilder(telemetryBuilder)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation(b =>
        {
            b.RecordException = true;
            b.Enrich = (activity, _, _) =>
            {
                if (activity.GetTagItem("user.id") != null)
                {
                    return;
                }

                var userId = activity.GetBaggageItem("user.id");

                if (userId != null)
                {
                    activity.AddTag("user.id", userId);
                }
            };
        }));

    // Logging
    builder.Logging.ClearProviders();

    builder.Host.UseSerilog((context, services, configuration) => configuration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Disable request logging - requests are traced by OpenTelemetry
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.With<TraceLogEnricher>(),
        writeToProviders: true);

    builder.Logging.AddOpenTelemetry(b =>
    {
        b.SetResourceBuilder(telemetryBuilder);
        b.ParseStateValues = true;
        b.IncludeFormattedMessage = true;

        b.AttachLogsToActivityEvent();
    });

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}