using Microsoft.AspNetCore.Diagnostics;

namespace PaymentGateway.Extensions;

public static class ExceptionHandlerExtensions
{
    public static void UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature is not null)
                {
                    // TODO: Add logging here
                    //contextFeature.Error
                }

                await context.Response.WriteAsync("Internal Server Error.");
            });
        });
    }
}