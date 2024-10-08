﻿using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Multiplify.Application.SignalR;
using System.Reflection;

namespace Multiplify.Application.Extensions;
public static class MiddlewareInitializer
{
    public static void ConfigureApplication(this WebApplication app)
    {
        RegisterSwagger(app);
        RegisterMiddlewares(app);
    }

    private static void RegisterSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    private static void RegisterMiddlewares(WebApplication app)
    {
        app.UseCors(opt =>
        {
            opt.AllowAnyOrigin();
            opt.AllowAnyHeader();
            opt.AllowAnyMethod();
        });


        app.UseStaticFiles();
        app.UseRouting();

        app.MapGet("/", async (context) => await context.Response
        .WriteAsync(PageTemplates.PageTemplates.GetIndexPage(Assembly
        .GetExecutingAssembly().GetName().Name, "relogosquare.jpg")));

        app.UseEndpoints(endpoints =>
        {
            _ = endpoints.MapHub<NotificationHub>("/notificationHub");
        });

        app.UseHangfireDashboard("/hangfire");
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
