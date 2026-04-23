namespace smartHome.Api.Extensions;

using smartHome.Core.Interfaces;
using smartHome.Infrastructure.Repositories;
using smartHome.Api.Services;

public static class ServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDeviceService, DeviceService>();
        //services.AddSingleton<IDeviceService, DeviceService>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
    }
}
