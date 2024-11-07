namespace CutEditor.Dialogs;

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Services;
using Wpf.Ui.Controls;
using static CutEditor.Model.Enums;

public partial class EasingPickerDialog : ContentDialog
{
    private readonly Vm vm;
    public EasingPickerDialog(Ease current, ContentPresenter? dialogHost) : base(dialogHost)
    {
        this.vm = new Vm(current);
        this.DataContext = this.vm;

        this.InitializeComponent();
    }

    public Ease Result => this.vm.Selected.Ease;

    //// --------------------------------------------------------------------------------------------

    public sealed class Vm : ObservableObject
    {
        private readonly EasingGraph.Graph unset;
        private EaseCategory category = EaseCategory.All;
        private EasingGraph.Graph selected;

        public Vm(Ease current)
        {
            this.CollectionView = CollectionViewSource.GetDefaultView(EasingGraph.Instance.Graphs);
            this.CollectionView.SortDescriptions.Add(new SortDescription(nameof(EasingGraph.Graph.VisualOrder), ListSortDirection.Ascending));
            this.CollectionView.Filter = e =>
            {
                if (e is not EasingGraph.Graph graph)
                {
                    return false;
                }

                return graph.IsInCategory(this.Category);
            };

            this.selected = this.CollectionView.Cast<EasingGraph.Graph>().FirstOrDefault(e => e.Ease == current)
                ?? throw new Exception($"invalid easing value: {current}");

            this.unset = EasingGraph.Instance.Graphs.First(e => e.Ease == Ease.Unset);
        }

        public ICollectionView CollectionView { get; set; }
        public EaseCategory Category
        {
            get => this.category;
            set
            {
                if (this.SetProperty(ref this.category, value))
                {
                    this.CollectionView.Refresh();
                }
            }
        }

        public EasingGraph.Graph Selected
        {
            get => this.selected;
            // note: 선택된 값이 filter에 의해서 리스트에서 사라질 때 ICollectionView가 강제로 null을 set 합니다. 
            set => this.SetProperty(ref this.selected, value ?? this.unset);
        }
    }
}
