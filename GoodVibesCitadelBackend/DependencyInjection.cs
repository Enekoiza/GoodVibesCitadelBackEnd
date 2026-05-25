namespace GoodVibesCitadelBackend;

using ApplicationService;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Configuration;
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

        services.Configure<CharacterPasswordProtectionOptions>(
            configuration.GetSection(CharacterPasswordProtectionOptions.SectionName));

        services.AddScoped<IGetAllCharacters, EfGetAllCharacters>();
        services.AddDataProtection();
        services.AddScoped<ICharacterPasswordProtector, CharacterPasswordProtector>();
        services.AddScoped<IUpdatedCharacterInformation, EfUpdateCharacterInformation>();
        services.AddScoped<IUpdatePasswordTemporaryFlag, EfUpdatePasswordTemporaryFlag>();
        services.AddScoped<IAddNewEvent, EfAddNewEvent>();
        services.AddScoped<IGetAllEvents, EfGetAllEvents>();
        services.AddScoped<IGetAllPartyCompositions, EfGetAllPartyCompositions>();
        services.AddScoped<IGetAllPartyCompositions, EfGetAllPartyCompositions>();
        services.AddScoped<ICompositionValidation, CompositionValidation>();
        services.AddScoped<IAddNewEventPartyCombination, EfAddNewEventPartyCombination>();
        services.AddScoped<IUpdateEventDrops, EfUpdateEventDrops>();
        services.AddScoped<IGetBorrowedEventCharacterCredentials, EfGetBorrowedEventCharacterCredentials>();
        services.AddScoped<IGetAllEventsProcessor, GetAllEventsProcessor>();
        services.AddScoped<ISearchRecetas, EfSearchRecetas>();
        services.AddScoped<IGetRecetaById, EfGetRecetaById>();
        services.AddScoped<IGetRecetaMateriales, EfGetRecetaMateriales>();
        services.AddScoped<ILookupItemsByNames, EfLookupItemsByNames>();
        services.AddScoped<IGetCpWarehouse, EfGetCpWarehouse>();
        services.AddScoped<IAddCpWarehouseEntry, EfAddCpWarehouseEntry>();
        services.AddScoped<IUpdateCpWarehouseEntry, EfUpdateCpWarehouseEntry>();
        services.AddScoped<IDeleteCpWarehouseEntry, EfDeleteCpWarehouseEntry>();
        services.AddScoped<ISearchWarehouseCatalog, EfSearchWarehouseCatalog>();
        services.AddScoped<ISyncCpWarehouse, EfSyncCpWarehouse>();

        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 1;
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