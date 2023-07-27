using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace LafiamiAPI.Middlewares
{
    public static class GlobalExceptionHandlerExtension
    {
        //This method will globally handle logging unhandled execeptions.
        //It will respond json response for ajax calls that send the json accept header
        //otherwise it will redirect to an error page
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app
                                                    , ILogger logger
                                                    , bool respondWithJsonErrorDetails = false)
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    //============================================================
                    //Log Exception
                    //============================================================
                    Exception exception = context.Features.Get<IExceptionHandlerFeature>().Error;

                    logger.LogError(JsonSerializer.Serialize($@"{exception.Message}
                                         {Environment.NewLine}
                                         {exception.StackTrace}"));

                    string message = $@"{exception.Message}";

                    int status = (int)HttpStatusCode.InternalServerError;
                    Type exceptionType = exception.GetType();

                    if (exceptionType == typeof(UnauthorizedAccessException))
                    {
                        message = "Unauthorized Access";
                        status = (int)HttpStatusCode.Unauthorized;
                    }
                    else if (exceptionType == typeof(NotImplementedException))
                    {
                        message = "Not Implemented Action";
                        status = (int)HttpStatusCode.NotImplemented;
                    }
                    else if (exceptionType == typeof(WebsiteException))
                    {
                        message = exception.Message;
                        status = (int)HttpStatusCode.Conflict;
                    }
                    else
                    {
                        status = (int)HttpStatusCode.InternalServerError;
                    }

                    context.Response.StatusCode = status;
                    string json = JsonSerializer.Serialize(message);

                    logger.LogError(exception, message);
                    //============================================================
                    //Return response
                    //============================================================
                    context.Response.ContentType = "application/json; charset=utf-8";

                    if (!respondWithJsonErrorDetails)
                    {
                        json = JsonSerializer.Serialize("Unexpected Error");
                    }

                    System.Console.WriteLine("Error Logger Initialzed");
                    System.Console.WriteLine(json);
                    System.Console.WriteLine(exception);

                    await context.Response.WriteAsync(json, Encoding.UTF8);
                });
            });
        }
    }
}
