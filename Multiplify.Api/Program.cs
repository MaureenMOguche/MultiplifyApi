using Multiplify.Application.Extensions;
using Multiplify.Infrastructure;
using Multiplify.Infrastructure.SeedData;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterApplicationServices(builder, builder.Configuration);
builder.Services.AddPersistenceServices();

_ = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var app = builder.Build();
app.ConfigureApplication();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
if (scopeFactory != null)
{
    using var scope = scopeFactory.CreateScope();
    var provider = scope.ServiceProvider;
    await ApplicationSeedData.InitializeAsync(provider);
}

app.Run();
