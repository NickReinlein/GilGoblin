using Microsoft.AspNetCore.Http;
using Prometheus;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GilGoblin.Api.Middleware;

public class RequestInfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Histogram _httpRequestDurationHistogram;

    public RequestInfoMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        _httpRequestDurationHistogram = Metrics.CreateHistogram(
            "http_request_duration_seconds",
            "Duration of HTTP requests",
            new HistogramConfiguration { LabelNames = new[] { "method", "status_code" } });
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalSeconds;

            _httpRequestDurationHistogram
                .WithLabels(context.Request.Method, context.Response.StatusCode.ToString())
                .Observe(duration);
        }
    }
}