namespace Du.Presentation.Extensions;

using System;
using System.Windows.Markup;

public sealed class EnumValuesExtension : MarkupExtension
{
    private readonly Type enumType;

    public EnumValuesExtension(Type enumType)
    {
        if (enumType.IsEnum == false)
        {
            throw new ArgumentNullException($"{nameof(enumType)} is not enum type.");
        }

        this.enumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(this.enumType);
    }
}
