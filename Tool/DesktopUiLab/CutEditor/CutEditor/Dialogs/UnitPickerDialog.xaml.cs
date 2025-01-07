namespace CutEditor.Dialogs;

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;
using static NKM.NKMOpenEnums;

public partial class UnitPickerDialog : ContentDialog
{
    private readonly ViewModel viewModel = new();
    public UnitPickerDialog(ContentPresenter? dialogHost) : base(dialogHost)
    {
        this.DataContext = this.viewModel;
        this.InitializeComponent();
    }

    public string? SelectedValue => this.viewModel.FinalValue;
    public Unit? SelectedUnit => this.viewModel.SelectedUnit;

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.viewModel.FinalValue is null)
        {
            this.InfoBarWarning.Visibility = Visibility.Visible;
            return;
        }

        base.OnButtonClick(button);
    }

    private sealed class ViewModel : ObservableObject
    {
        private readonly ISearchableCollection<Unit> filteredList;
        private string searchKeyword = string.Empty;
        private Unit? selectedUnit;
        private UnitIdConst? selectedConst;

        public ViewModel()
        {
            var candidates = Unit.Values.Where(e => e.EnableForCutscene());
            this.filteredList = App.Current.Services.GetRequiredService<ISearchableCollectionProvider>().Build(candidates);
        }

        public IEnumerable FilteredFiles => this.filteredList.List;
        public Unit? SelectedUnit
        {
            get => this.selectedUnit;
            set => this.SetProperty(ref this.selectedUnit, value);
        }

        public UnitIdConst? SelectedConst
        {
            get => this.selectedConst;
            set => this.SetProperty(ref this.selectedConst, value);
        }

        public string? FinalValue { get; private set; }

        public string SearchKeyword
        {
            get => this.searchKeyword;
            set => this.SetProperty(ref this.searchKeyword, value);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.PropertyName)
            {
                case nameof(this.SearchKeyword):
                    this.filteredList.Refresh(this.SearchKeyword);
                    break;

                case nameof(this.SelectedConst):
                    if (this.selectedConst is not null)
                    {
                        this.FinalValue = this.selectedConst.ToString();
                    }

                    break;

                case nameof(this.SelectedUnit):
                    if (this.selectedUnit is not null)
                    {
                        this.FinalValue = this.selectedUnit.StrId;
                    }

                    break;
            }
        }
    }
}
