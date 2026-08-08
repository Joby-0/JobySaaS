
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Configuration.Extensions;

public static class SecretsExtensions
{
    const string _appsettingfile = "appsettings.json";

    //to use either user secrets or azure key vault depending on UseAzureKeyVault tag in appsettings.json
    //Azure key vault access parameters location are set in <AzureProjectSettings> tag in the csproj file
    //User secret id is set in <UserSecretsId>
    public static IConfigurationBuilder AddSecrets(this IConfigurationBuilder config, string appFolder)
    {
        // current directory is either the application or the dbContext when running migrations
        var currentDir = Directory.GetCurrentDirectory();
#if DEBUG
        config.SetBasePath(Path.Combine(currentDir, "..", appFolder))
                .AddJsonFile(_appsettingfile, optional: true, reloadOnChange: true);
#else        
        config.SetBasePath(currentDir)
                .AddJsonFile(_appsettingfile, optional: true, reloadOnChange: true);
#endif
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

        // Build a temporary configuration to read the SecretStorage setting
        var tempConfig = config.Build();
        string secretStorage = tempConfig.GetValue<string>("ApplicationSecrets:SecretStorage");
        Console.WriteLine($"Using Secret Storage: {secretStorage}");

        //to use either user secrets or azure key vault depending on SecretStorage tag in appsettings.json
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var assembly = System.Reflection.Assembly.Load("Configuration");

            if (secretStorage == "UserSecrets")
            {
                // In development, we use user secrets
                Console.WriteLine("Using User Secrets in Development environment.");

                // Load user secrets from Configuration project assembly
                config.AddUserSecrets(assembly);

                // Read the UserSecretsId programmatically
                var userSecretsIdAttribute = assembly.GetCustomAttributes(typeof(UserSecretsIdAttribute), false)
                    .FirstOrDefault() as UserSecretsIdAttribute;
                var userSecretsId = userSecretsIdAttribute?.UserSecretsId;
                Console.WriteLine($"Using User Secrets ID: {userSecretsId}");
            }
            
       

        return config;
    }
    }

