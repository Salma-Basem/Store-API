using Store.Service.HandleResponses;
using System.Net;
using System.Security.AccessControl;
using System.Text.Json;

namespace Store.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _enivronment;
        private readonly ILogger _logger;
        public ExceptionMiddleware(RequestDelegate next,
            IHostEnvironment enivronment,
            ILogger<ExceptionMiddleware>logger  )
        {
            _next = next;  
            _enivronment=enivronment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _enivronment.IsDevelopment()
                    ? new CustomException((int)HttpStatusCode.InternalServerError,ex.Message,ex.StackTrace)
                    : new CustomException((int)HttpStatusCode.InternalServerError)  ;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
