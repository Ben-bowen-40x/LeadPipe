using LeadPipe.Translation.Data;
using LeadPipe.Translation.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace LeadPipe.Translation;

public static class InjectTranslation
{
    public static IServiceCollection AddTranslation(this IServiceCollection services)
    {
        // Format: services.AddScoped<Interface, Class>();
        services.AddScoped<IDtoToEntity, DtoToEntity>();
        services.AddScoped<IDtoToVo, DtoToVo>();
        services.AddScoped<IEntityToDto, EntityToDto>();
        services.AddScoped<IEntityToVo, EntityToVo>();
        services.AddScoped<IVoToDto, VoToDto>();
        services.AddScoped<IVoToEntity, VoToEntity>();
        services.AddScoped<IDateTimeTranslate, DateTimeTranslate>();
        return services;
    }
}
