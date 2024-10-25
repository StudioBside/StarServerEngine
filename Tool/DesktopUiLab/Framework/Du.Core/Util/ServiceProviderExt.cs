namespace Du.Core.Util;

using System;

public static class ServiceProviderExt
{
    public static T GetServiceNotNull<T>(this IServiceProvider provider) where T : class
    {
        var result = provider.GetService(typeof(T)) as T;
        return result ?? throw new InvalidOperationException($"Service not found: {typeof(T).Name}");
    }
}
