using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Multiplify.Application.Middlewares;
using Serilog;
using Multiplify.Application.Config;
using Asp.Versioning;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Multiplify.Application.Models;
using Multiplify.Application.Contracts.Services;
using Multiplify.Application.ServiceImplementations;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using Multiplify.Application.ServiceImplementations.Helpers;
using Microsoft.AspNetCore.Http;

namespace Multiplify.Application.Extensions;
public static class ServiceExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services,
        WebApplicationBuilder builder,
        IConfiguration config)
    {
        SetupControllers(services);
        SetupSwagger(services);
        ConfigureSerilog(builder);
        SetupAuthentication(services);
        BindConfigurations(services);
        AddApiVersioning(services);
        RegisterFilters(services);
        AddServiceDependencies(services);
        RegisterFluentValidation(services);
        AddOptions(services);
        SetupHangfire(services, config);
        OtherSetup(services);
    }

    private static void OtherSetup(IServiceCollection services)
    {
        var scope = services.BuildServiceProvider().CreateScope();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        TokenProviders.Initialize(memoryCache);
        UserHelper.Configure(scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>());
    }

    private static void SetupHangfire(IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(opt =>
        {
            opt.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"));
        });
        
        services.AddHangfireServer();
    }

    private static void AddOptions(IServiceCollection services)
    {
        services.AddOptions<JwtSettings>()
            .BindConfiguration(nameof(JwtSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailSettings>()
            .BindConfiguration(nameof(EmailSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void AddServiceDependencies(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        //services.AddScoped<IHttpService, HttpService>();

        services.AddScoped<IWaitlistService, WaitlistService>();
        services.AddSingleton<IMessagingService, MessagingService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEntreprenuerService, EntreprenuerService>();
        services.AddScoped<IFunderService, FunderService>();
        services.AddScoped<IPhotoService,  PhotoService>();

        //services.AddScoped<IFunderService, IFunderService>();
    }

    private static void SetupAuthentication(IServiceCollection services)
    {
        var jwtSettings = services.BuildServiceProvider().GetService<IOptions<JwtSettings>>()?.Value;
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
            };
        });

    }

    private static void RegisterFilters(IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalErrorHandling>();
        services.AddProblemDetails();
    }

    private static void SetupSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlfile = "Multiplify.Api.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
            options.IncludeXmlComments(xmlPath);

            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Multiplify", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static void RegisterFluentValidation(IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            var apiReader = new UrlSegmentApiVersionReader();
            options.ApiVersionReader = apiReader;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void BindConfigurations(IServiceCollection services)
    {
        services.AddOptions<JwtSettings>()
            .BindConfiguration(nameof(JwtSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    private static void SetupControllers(IServiceCollection services)
    {
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = ModelValidation.ValidateModel;
        });
    }
}
