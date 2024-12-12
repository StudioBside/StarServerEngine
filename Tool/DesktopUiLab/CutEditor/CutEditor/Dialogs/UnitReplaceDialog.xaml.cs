namespace CutEditor.Dialogs;

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;

public partial class UnitReplaceDialog : ContentDialog
{
    private readonly ViewModel viewModel;
    public UnitReplaceDialog(ContentPresenter? dialogHost, IReadOnlyList<Unit> prevUnits) : base(dialogHost)
    {
        this.viewModel = new ViewModel(prevUnits);
        this.DataContext = this.viewModel;

        this.InitializeComponent();
    }

    public Unit PrevUnit => this.viewModel.PrevUnit;
    public Unit AfterUnit => this.viewModel.AfterUnit;

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.viewModel.Validate() == false)
        {
            this.InfoBarWarning.Visibility = Visibility.Visible;
            return;
        }

        base.OnButtonClick(button);
    }

    private sealed class ViewModel : ObservableObject
    {
        private readonly ISearchableCollection<Unit> prevUnits;
        private readonly ISearchableCollection<Unit> afterUnits;
        private string prevSearchKeyword = string.Empty;
        private string afterSearchKeyword = string.Empty;
        private Unit prevUnit;
        private Unit afterUnit;
        private string errorMessage = string.Empty;

        public ViewModel(IReadOnlyList<Unit> prevUnits)
        {
            this.prevUnit = prevUnits[0];
            this.afterUnit = prevUnits[0];

            this.prevUnits = App.Current.Services.GetRequiredService<ISearchableCollectionProvider>().Build(prevUnits);
            var candidates = Unit.Values.Where(e => e.EnableForCutscene());
            this.afterUnits = App.Current.Services.GetRequiredService<ISearchableCollectionProvider>().Build(candidates);
        }

        public IEnumerable PrevUnits => this.prevUnits.List;
        public IEnumerable AfterUnits => this.afterUnits.List;

        public string PrevSearchKeyword
        {
            get => this.prevSearchKeyword;
            set => this.SetProperty(ref this.prevSearchKeyword, value);
        }

        public string AfterSearchKeyword
        {
            get => this.afterSearchKeyword;
            set => this.SetProperty(ref this.afterSearchKeyword, value);
        }

        public Unit PrevUnit
        {
            get => this.prevUnit;
            set => this.SetProperty(ref this.prevUnit, value);
        }

        public Unit AfterUnit
        {
            get => this.afterUnit;
            set => this.SetProperty(ref this.afterUnit, value);
        }

        public string ErrorMessage
        {
            get => this.errorMessage;
            set => this.SetProperty(ref this.errorMessage, value);
        }

        internal bool Validate()
        {
            if (this.prevUnit is null)
            {
                this.ErrorMessage = "변경할 유닛이 올바르지 않습니다.";
                return false;
            }

            if (this.afterUnit is null)
            {
                this.ErrorMessage = "변경될 유닛이 올바르지 않습니다.";
                return false;
            }

            if (this.prevUnit == this.afterUnit)
            {
                this.ErrorMessage = "변경할 유닛과 변경될 유닛이 같습니다.";
                return false;
            }

            return true;
        }

        //// --------------------------------------------------------------------------------------------

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.PropertyName)
            {
                case nameof(this.PrevSearchKeyword):
                    this.prevUnits.Refresh(this.prevSearchKeyword);
                    break;

                case nameof(this.AfterSearchKeyword):
                    this.afterUnits.Refresh(this.afterSearchKeyword);
                    break;
            }
        }
    }
}
