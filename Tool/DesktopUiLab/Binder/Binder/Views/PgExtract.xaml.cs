﻿namespace Binder.Views;

using System.Windows.Controls;
using Binder.ViewModel;
using Du.Core.Util;

public partial class PgExtract : Page
{
    public PgExtract()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmExtract>();
    }
}