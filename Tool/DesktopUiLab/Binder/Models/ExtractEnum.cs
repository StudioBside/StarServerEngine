namespace Binder.Models;

using System.Collections.Generic;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;
using static Binder.Models.Enums;

public sealed class ExtractEnum : ObservableObject
{
    public ExtractEnum(JsonElement element)
    {
    }

    public override string ToString()
    {
        return string.Empty;
    }

    //// --------------------------------------------------------------------------------------------
}
