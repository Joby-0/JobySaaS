using Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class StripeExtensions
{
    public static IServiceCollection AddStripe(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        
        serviceCollection.Configure<StripeOptions>(options => configuration.GetSection(StripeOptions.Position).Bind(options));


        return serviceCollection;
    }
}