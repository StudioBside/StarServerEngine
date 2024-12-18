namespace CutEditor.Selectors;

using System.Windows;
using System.Windows.Controls;
using CutEditor.Model.CutSearch.Conditions;

public sealed class CutSearchConditionDtSelector : DataTemplateSelector
{
    public DataTemplate TextContains { get; set; } = null!;
    public DataTemplate UnitMatch { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            ConditionTextContains => this.TextContains,
            ConditionUnitMatch => this.UnitMatch,
            _ => base.SelectTemplate(item, container),
        };
    }
}
