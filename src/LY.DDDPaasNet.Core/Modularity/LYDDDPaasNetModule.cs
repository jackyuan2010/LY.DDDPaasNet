namespace LY.DDDPaasNet.Core.Modularity;

public abstract class LYDDDPaasNetModule : IModule
{
    public virtual Task PreConfigureServicesAsync(ServiceConfigurationContext context)
    {
        PreConfigureServices(context);
        return Task.CompletedTask;
    }

    public virtual void PreConfigureServices(ServiceConfigurationContext context)
    {
    }

    public virtual Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        ConfigureServices(context);
        return Task.CompletedTask;
    }

    public virtual void ConfigureServices(ServiceConfigurationContext context)
    {
    }

    public virtual Task PostConfigureServicesAsync(ServiceConfigurationContext context)
    {
        PostConfigureServices(context);
        return Task.CompletedTask;
    }

    public virtual void PostConfigureServices(ServiceConfigurationContext context)
    {
    }
}