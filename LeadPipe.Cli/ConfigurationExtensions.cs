using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LeadPipe.Cli;

internal static class ConfigurationExtensions
{
    internal static void AddSecrets(this HostBuilderContext context, IConfigurationBuilder config)
    {
        // Load prod secrets
        if (!context.HostingEnvironment.IsDevelopment())
        {
            var containerPath = "/app/secrets.json";
            var localDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".leadpipe");
            var localPath = Path.Combine(localDir, "secrets.json");

            config
                .AddJsonFile(localPath, optional: true)
                .AddJsonFile(containerPath, optional: true);

            // At least one must exist
            var hasContainer = File.Exists(containerPath);
            var hasLocal = File.Exists(localPath);
            if (!hasContainer && !hasLocal)
            {
                throw new InvalidOperationException(
                    $"No secrets file found. Expected either:\n" +
                    $"- {containerPath}\n" +
                    $"- {localPath}");
            }
        }
    }
}