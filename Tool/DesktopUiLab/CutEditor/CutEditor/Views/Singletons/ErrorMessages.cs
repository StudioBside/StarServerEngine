namespace CutEditor.Views.Singletons;

using System;
using System.Windows.Markup;
using Shared.Templet.Errors;

public sealed class ErrorMessages : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return ErrorMessageController.Instance;
    }
}
