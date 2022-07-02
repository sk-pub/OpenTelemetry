using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Api2
{
    public class TraceLogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.RemovePropertyIfPresent("RequestId");
            logEvent.RemovePropertyIfPresent("RequestPath");
            logEvent.RemovePropertyIfPresent("ActionId");
            logEvent.RemovePropertyIfPresent("ActionName");
            logEvent.RemovePropertyIfPresent("ConnectionId");

            var traceId = Activity.Current?.TraceId;
            if (traceId != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId.ToString()));
            }

            var spanId = Activity.Current?.SpanId;
            if (traceId != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", spanId.ToString()));
            }
        }
    }
}
