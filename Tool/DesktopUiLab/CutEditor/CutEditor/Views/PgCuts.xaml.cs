namespace CutEditor.Views;

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CutEditor.ViewModel;
using Du.Core.Util;

public sealed partial class PgCuts : Page
{
    public PgCuts()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmCuts>();
    }
}
