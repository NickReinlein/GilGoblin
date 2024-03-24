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
    private readonly Gauge _cpuUsageGauge;

    public RequestInfoMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        _httpRequestDurationHistogram = Metrics.CreateHistogram(
            "http_request_duration_seconds",
            "Duration of HTTP requests",
            new HistogramConfiguration { LabelNames = new[] { "method", "status_code" } });

        _cpuUsageGauge = Metrics.CreateGauge("cpu_usage_percentage", "CPU usage percentage");
    }

    public async Task Invoke(HttpContext context, CpuMetrics cpuMetrics)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var cpuUsage = CpuMetrics.GetCpuUsage();
            _cpuUsageGauge.Set(cpuUsage);

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