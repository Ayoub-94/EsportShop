using System.Net;
using System.Text.Json;

namespace EsportShop.Api.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Passe la requête au middleware suivant (le contrôleur, etc.)
                await _next(context);
            }
            catch (Exception ex)
            {
                // Intercepte n'importe quelle erreur non gérée dans l'application
                _logger.LogError(ex, "Une erreur non gérée est survenue : {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/ json";

            // Déterminer le code HTTP et le message selon le type d'exception

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Une erreur interne est survenue sur le serveur";

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    message = exception.Message;
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Accès non autorisé";
                    break;
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            // Format de réponse propre et uniforme (inspiré du standard ProblemDetails)
            var response = new
            {
                status = context.Response.StatusCode,
                error = statusCode.ToString(),
                message = message,

                // On peut afficher les détails techniques uniquement en mode Développement
                detail = _env.IsDevelopment() ? exception.StackTrace : null
            };

            var options = new JsonSerializerOptions {  PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
