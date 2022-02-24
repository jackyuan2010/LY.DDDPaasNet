using Microsoft.Extensions.DependencyInjection;

namespace LY.DDDPaasNet.Core.Modularity;

public class ServiceConfigurationContext
{
    public IServiceCollection Services { get; }

    public ServiceConfigurationContext(IServiceCollection services)
    {
        Services = services;
    }
}