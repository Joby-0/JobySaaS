using Configuration.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DbContext;

namespace DbContext.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddMainDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
        });

        return services;
    }

    public static IServiceCollection AddJwtTokenService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Position));

        var jwtOptions = configuration.GetSection(JwtOptions.Position).Get<JwtOptions>();
        if (jwtOptions == null)
            throw new InvalidOperationException("JwtConfig section is missing");

        services.AddScoped<JWTService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidateLifetime = jwtOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    ValidAudience = jwtOptions.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.IssuerSigningKey))
                };
            });

        return services;
    }
}
