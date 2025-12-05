using LeadPipe.Application.Manager;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LeadPipe.Application;

public static class InjectApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Format: services.AddScoped<Interface, Class>();  

        // Add update managers
        RegisterManagers(services);

        // Add managers singly
        services.AddScoped<IFileRWManager, FileRWManager>();
        services.AddScoped<IPlumbingAssociationManager, PlumbingAssociationManager>();

        return services;
    }
    private static void RegisterManagers(IServiceCollection services)
    {
        Assembly? assembly = Assembly.GetAssembly(typeof(InjectApplication));
        if (assembly is null)
            return;

        IEnumerable<Type> managers = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                typeof(IUpdateManager).IsAssignableFrom(t)
            );

        foreach (Type? managerType in managers)
        {
            // Find the specific interface
            Type? serviceInterface = managerType
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i != typeof(IUpdateManager) &&
                    typeof(IUpdateManager).IsAssignableFrom(i));

            if (serviceInterface is null)
                continue;

            services.AddScoped(serviceInterface, managerType);
        }
    }

}
