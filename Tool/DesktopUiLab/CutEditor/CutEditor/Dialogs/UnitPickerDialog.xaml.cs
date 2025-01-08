namespace CutEditor.Dialogs;

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Model.Detail;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;
using static NKM.NKMOpenEnums;

public partial class UnitPickerDialog : ContentDialog
{
    private readonly ViewModel viewModel;

    public UnitPickerDialog(ContentPresenter? dialogHost, bool enableIdConst) : base(dialogHost)
    {
        this.viewModel = new ViewModel(enableIdConst);
        this.DataContext = this.viewModel;
        this.InitializeComponent();
    }

    public UnitVariant ResultVariant => this.viewModel.Result;
    public Unit? ResultUnit => this.viewModel.SelectedUnit;

    public void SetCurrentValue(UnitVariant currentValue)
    {
        this.viewModel.SelectedUnit = currentValue?.Unit;
        this.viewModel.SelectedConst = currentValue?.UnitIdConst;
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.viewModel.HasSelection == false)
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

        public ViewModel(bool enableIdConst)
        {
            var candidates = Unit.Values.Where(e => e.EnableForCutscene());
            this.filteredList = App.Current.Services.GetRequiredService<ISearchableCollectionProvider>().Build(candidates);
            this.EnableIdConst = enableIdConst;
        }

        public bool EnableIdConst { get; }
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

        public bool HasSelection => this.SelectedUnit != null || this.SelectedConst != null;
        public UnitVariant Result => new UnitVariant(this.selectedUnit, this.selectedConst);

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

                case nameof(this.SelectedUnit):
                    if (this.selectedUnit != null)
                    {
                        this.SelectedConst = null;
                    }

                    break;

                case nameof(this.SelectedConst):
                    if (this.selectedConst != null)
                    {
                        this.SelectedUnit = null;
                    }

                    break;
            }
        }
    }
}
