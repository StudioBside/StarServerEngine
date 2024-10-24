namespace CutEditor.Selectors;

using System.Windows;
using System.Windows.Controls;
using CutEditor.Model;
using CutEditor.ViewModel;
using Du.Core.Util;

internal sealed class CutItemTempletSelector : DataTemplateSelector
{
    public DataTemplate SummaryTemplate { get; set; } = null!;
    public DataTemplate NormalTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var vmCut = (VmCut)item;
        var vmCuts = VmCuts.LastInstance;
        if (vmCuts is null)
        {
            return base.SelectTemplate(item, container);
        }

        return vmCuts.ShowSummary ? this.SummaryTemplate : this.NormalTemplate;
    }
}
