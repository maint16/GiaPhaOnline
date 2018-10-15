using System.Net;
using AppModel.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Models;

namespace Main.Extensions
{
    public static class AppExceptionHandlerExtension
    {
        public static void UseCustomizedExceptionHandler(this IApplicationBuilder app, IHostingEnvironment env)
        {
            // Use exception handler for errors handling.
            app.UseExceptionHandler(options =>
            {
                options.Run(
                    async context =>
                    {
                        // Mark the response status as 500.
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                        // No exception handler feature has been found.
                        if (exceptionHandlerFeature == null || exceptionHandlerFeature.Error == null)
                            return;

                        // Initialize response asynchronously.
                        var apiResponse = new ApiResponse(exceptionHandlerFeature.Error.Message);
                        var contractResolver = new DefaultContractResolver();
                        contractResolver.NamingStrategy = new CamelCaseNamingStrategy();

                        var jsonSerializerSettings = new JsonSerializerSettings();
                        jsonSerializerSettings.ContractResolver = contractResolver;

                        var szApiResponse = JsonConvert.SerializeObject(apiResponse, jsonSerializerSettings);
                        if (exceptionHandlerFeature.Error is ApiException)
                        {
                            var apiException = exceptionHandlerFeature.Error as ApiException;
                            context.Response.StatusCode = (int) apiException.Status;
                        }
                        await context.Response.WriteAsync(szApiResponse).ConfigureAwait(false);
                    });
            });
        }
    }
}