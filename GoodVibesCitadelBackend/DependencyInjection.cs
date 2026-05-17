namespace GoodVibesCitadelBackend;

using ApplicationService;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Ef;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IGetAllCharacters, EfGetAllCharacters>();
        services.AddScoped<IUpdatedCharacterInformation, EfUpdateCharacterInformation>();
        services.AddScoped<IUpdatePasswordTemporaryFlag, EfUpdatePasswordTemporaryFlag>();
        services.AddScoped<IAddNewEvent, EfAddNewEvent>();
        services.AddScoped<IGetAllEvents, EfGetAllEvents>();
        services.AddScoped<IGetAllPartyCompositions, EfGetAllPartyCompositions>();
        services.AddScoped<IGetAllPartyCompositions, EfGetAllPartyCompositions>();
        services.AddScoped<ICompositionValidation, CompositionValidation>();

        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        return services;
    }
}