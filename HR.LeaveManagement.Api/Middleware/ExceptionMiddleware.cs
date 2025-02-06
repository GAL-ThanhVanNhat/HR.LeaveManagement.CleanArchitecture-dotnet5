using HR.LeaveManagement.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Api.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            } catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new ErrorDeatils
            {
                ErrorMessage = exception.Message,
                ErrorType = "Failure"
            });

            switch (exception)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.Errors);
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                default:
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }

    public class ErrorDeatils
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
