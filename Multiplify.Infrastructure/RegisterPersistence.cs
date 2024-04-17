using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Multiplify.Application.Contracts.Repository;
using Multiplify.Domain;
using Multiplify.Infrastructure.Repositories;

namespace Multiplify.Infrastructure;
public static class RegisterPersistence
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddIdentity<AppUser, IdentityRole>(opt =>
        {
            opt.Lockout.MaxFailedAccessAttempts = 5;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

    }
}
