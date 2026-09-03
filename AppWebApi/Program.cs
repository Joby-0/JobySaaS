using DbContext;
using DbContext.Extensions;
using DbRepos;
using Services;
using Configuration.Extensions;
using Microsoft.EntityFrameworkCore;
using Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

//adding support for several secret sources and database sources
//to use either user secrets or azure key vault depending on UseAzureKeyVault tag in appsettings.json
builder.Configuration.AddSecrets( "AppWebApi");
builder.Services.AddMemoryCache();

builder.Services.AddJwtTokenService(builder.Configuration);
builder.Services.AddMainDbContext(builder.Configuration);
builder.Services.AddEncryptions(builder.Configuration);

builder.Services.AddStripe(builder.Configuration);
builder.Services.AddScoped<IStripeService, StripeService>();


builder.Services.AddScoped<IYoutubeService, YoutubeService>();
builder.Services.AddScoped<AuthDbRepo>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<OrganizationDbRepo>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<SubscriptionDbRepo>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<UserDbRepo>();
builder.Services.AddScoped<InvitationDbRepo>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<SocialAccountDbRepo>();
builder.Services.AddScoped<ISocialAccountService, SocialAccountService>();
builder.Services.AddScoped<MediaDbRepo>();
builder.Services.AddScoped<IMediaService, MediaService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ApiReference",
        Version = "v1",
        Description = "Small ASP.NET Core Web API reference implementation that preserves the layered structure of the original solution."
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AllMedia API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "ApiReference is running.");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

    var connected = await db.Database.CanConnectAsync();

    Console.WriteLine($"MySQL connected: {connected}");
}

app.Run();
