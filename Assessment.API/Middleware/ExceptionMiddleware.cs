using System.Net;
using System.Text.Json;
using Assessment.API.Common;

namespace Assessment.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. Path: {Path}", context.Request.Path);

                var (statusCode, code) = MapException(ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var message = GetSafeMessage(ex, statusCode);

                var details = _env.IsDevelopment() ? ex.ToString() : null;

                var payload = ApiResponse<object>.Fail(message);

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }

        private static (int StatusCode, string Code) MapException(Exception ex)
        {
            return ex switch
            {
                ArgumentException => ((int)HttpStatusCode.BadRequest, "BAD_REQUEST"),
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, "NOT_FOUND"),
                InvalidOperationException => ((int)HttpStatusCode.Conflict, "CONFLICT"),
                UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
                _ => ((int)HttpStatusCode.InternalServerError, "SERVER_ERROR")
            };
        }

        private static string GetSafeMessage(Exception ex, int statusCode)
        {
            if (statusCode == (int)HttpStatusCode.InternalServerError)
                return "An unexpected error occurred. Please try again.";

            return ex.Message;
        }
    }
}