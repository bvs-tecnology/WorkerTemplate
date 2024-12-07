using System.Net;
using System.Text;
using Domain.Common;
using Domain.Exceptions;
using Domain.SeedWork.Notification;
using Infra.Utils.Constants;
using Newtonsoft.Json;
using static System.String;

namespace API.Middlewares
{
    public class ControllerMiddleware(RequestDelegate next, ILogger<ControllerMiddleware> logger)
    {
        private static readonly bool IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        private string _body = Empty;
        public async Task InvokeAsync(HttpContext context, INotification notification)
        {
            try
            {
                _body = NotAllowedBodyLogging(context) ? "***" : await GetBody(context.Request);
                await next(context);
                LogInformation(context);
            }
            catch (NotificationException)
            {
                await HandleNotificationExceptionAsync(context, notification.Notifications);
            }
            catch (NotAllowedException)
            {
                await HandleNotAllowedException(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task<string> GetBody(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        #region HANDLERS
        private async Task HandleNotificationExceptionAsync(HttpContext context, List<NotificationModel> notifications)
        {
            var result = new GenericResponse<object>();
            notifications.ForEach(x => result.AddError(x.Message));
            UpdateContext(context, HttpStatusCode.BadRequest);
            var stringResponse = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(stringResponse);
            await LogError(context, stringResponse);
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new GenericResponse<object>();
            result.AddError(IsDevelopment ? exception.Message : RequestErrorResponseConstant.InternalError);
            UpdateContext(context, HttpStatusCode.InternalServerError);
            var stringResponse = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(stringResponse);
            await LogError(context, stringResponse, exception.Message);
        }
        private async Task HandleNotAllowedException(HttpContext context)
        {
            var result = new GenericResponse<object>();
            result.AddError(RequestErrorResponseConstant.NotAllowed);
            UpdateContext(context, HttpStatusCode.MethodNotAllowed);
            var stringResponse = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(stringResponse);
            await LogError(context, stringResponse);
        }
        private static void UpdateContext(HttpContext context, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
        }
        #endregion

        #region LOGGING
        private void LogInformation(HttpContext context)
        {
            switch (context.Request.Method)
            {
                case "GET" when !IsNullOrEmpty(context.Request.QueryString.ToString()):
                    logger.LogInformation(
                        "Request {method} {url} returned {statusCode}; query: {queryString}",
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        context.Request.QueryString
                    );
                    break;
                case "POST":
                case "PUT":
                case "PATCH":
                    logger.LogInformation(
                        "Request {method} {url} returned {statusCode}; body: {body}",
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        _body
                    );
                    break;
                default:
                    logger.LogInformation(
                        "Request {method} {url} returned {statusCode}",
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode
                    );
                    break;
            }
        }

        private async Task LogError(HttpContext context, string result, string? exception = null)
        {
            string fullMessage;
            switch (context.Request.Method)
            {
                case "GET" when !IsNullOrEmpty(context.Request.QueryString.ToString()):
                    fullMessage = exception == null ?
                        "Request {method} {url} returned {statusCode}; result: {result}; query: {queryString}" :
                        "Request {method} {url} returned {statusCode}; result: {result}; query: {queryString}; exception: {exeption}";
                    logger.LogError(
                        fullMessage,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        result,
                        context.Request.QueryString,
                        exception
                    );
                    break;
                case "POST":
                case "PUT":
                case "PATCH":
                    fullMessage = exception == null ?
                        "Request {method} {url} returned {statusCode}; result: {result}; body: {body}" :
                        "Request {method} {url} returned {statusCode}; result: {result}; body: {body}; exception: {exeption}";
                    logger.LogError(
                        fullMessage,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        result,
                        _body,
                        exception
                    );
                    break;
                default:
                    fullMessage = exception == null ?
                        "Request {method} {url} returned {statusCode}; result: {result}" :
                        "Request {method} {url} returned {statusCode}; result: {result}; exception: {exeption}";
                    logger.LogError(
                        fullMessage,
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        result,
                        exception
                    );
                    break;
            }
        }
        #endregion

        private static bool NotAllowedBodyLogging(HttpContext context)
        {
            var notAllowedRequests = new List<(string Path, string Method)>
            {
                ("/api/{controller}", "POST")
            };

            var requestPath = context.Request.Path.ToString().ToLower();
            var requestMethod = context.Request.Method.ToUpper();

            return notAllowedRequests.Any(r => r.Path == requestPath && r.Method == requestMethod);
        }
    }
}
