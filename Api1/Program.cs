using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

// Telemetry
// Define some important constants and the activity source
var serviceName = "Api1";
var serviceVersion = "1.0.0";
var serviceInstanceId = Environment.MachineName;

builder.Services.AddOpenTelemetryTracing(b =>
{
    b
    .AddConsoleExporter()
    .AddJaegerExporter(b => b.Endpoint = new Uri("http://localhost:6831"))
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion, serviceInstanceId: serviceInstanceId))
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

            const string userId = "user-1";
            activity.AddBaggage("user.id", userId);
            activity.AddTag("user.id", userId);
        };
    });
});

// Logging
builder.Logging.ClearProviders();

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
