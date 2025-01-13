namespace CutEditor.Services;

using System;
using System.Windows.Markup;
using CutEditor.ViewModel;

public sealed class DataExposure : MarkupExtension
{
    public string ExportRoot => VmGlobalState.ExportRoot;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new DataExposure();
    }
}
