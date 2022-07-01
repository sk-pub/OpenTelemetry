# Distributed tracing and logging with OpenTelemetry

## Getting started

1. Make sure that Docker is installed
2. Run Jaeger
```
docker run -d --name jaeger \
  -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \
  -e COLLECTOR_OTLP_ENABLED=true \
  -p 6831:6831/udp \
  -p 6832:6832/udp \
  -p 5778:5778 \
  -p 16686:16686 \
  -p 4317:4317 \
  -p 4318:4318 \
  -p 14250:14250 \
  -p 14268:14268 \
  -p 14269:14269 \
  -p 9411:9411 \
  jaegertracing/all-in-one:1.35
  ```
  3. Start the Api1 project
  4. Open Api1 with http://localhost:5020/WeatherForecast
  5. The request should fail with an HTTP exception because Api2 isn't started
  6. Observe the trace with the exception in Jaeger UI at http://localhost:16686/