namespace Shared.Templet.Base;

using System;
using System.Collections.Generic;
using System.Reflection;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;

internal static class TempletContainerUtil
{
    private static readonly Type OpenedContainerType = typeof(TempletContainer<>);
    private static readonly Type[] AllTempletTypes;
    private static readonly Type[] ContainerTypes;

    static TempletContainerUtil()
    {
        // 이 클래스가 정의된 assembly를 획득.
        var assembly = Assembly.GetExecutingAssembly();

        AllTempletTypes = assembly.GetTypes()
            .Where(t => typeof(ITemplet).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .ToArray();

        ContainerTypes = AllTempletTypes
            .Select(templetType => OpenedContainerType.MakeGenericType(templetType))
            .ToArray();
    }

    public static void InvokeJoin()
    {
        foreach (var containerType in ContainerTypes)
        {
            var methodInfo = containerType.GetMethod("Join", BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo != null)
            {
                methodInfo.Invoke(obj: null, parameters: null);
            }
        }
    }

    public static void InvokeValidate()
    {
        foreach (var containerType in ContainerTypes)
        {
            var methodInfo = containerType.GetMethod("Validate");
            if (methodInfo != null)
            {
                methodInfo.Invoke(obj: null, parameters: null);
            }
        }
    }
}
