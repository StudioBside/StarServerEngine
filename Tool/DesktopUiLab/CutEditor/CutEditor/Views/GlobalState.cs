namespace CutEditor.Views;

using System;
using System.Windows.Markup;
using CutEditor.ViewModel;

public sealed class GlobalState : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return VmGlobalState.Instance;
    }
}
