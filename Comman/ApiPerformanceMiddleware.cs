using System.Diagnostics;

namespace WebApiNew.Comman
{
    public class ApiPerformanceMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ApiPerformanceMiddleware> _logger;

        public ApiPerformanceMiddleware(RequestDelegate next, ILogger<ApiPerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {

            var sw=Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            if (context.Request.Path.StartsWithSegments("/api") && sw.ElapsedMilliseconds >=100)
            {
                _logger.LogWarning(
                    "Slow API: {Method} {Path} took {ElapsedMs} ms, status {StatusCode}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    sw.ElapsedMilliseconds,
                    context.Response?.StatusCode
                );
            }
        }

    }
}