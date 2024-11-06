namespace CutEditor.Dialogs.BgFadeEditor;

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DrawingColor = System.Drawing.Color;

public partial class ColorEditor : UserControl
{
    private readonly Vm viewModel = new Vm();

    public ColorEditor()
    {
        this.DataContext = this.viewModel;
        this.InitializeComponent();
    }

    public void Initialize(DrawingColor modelColor, bool isEnabled)
    {
        this.viewModel.Color = Color.FromArgb(modelColor.A, modelColor.R, modelColor.G, modelColor.B);
        this.IsEnabled = isEnabled;
    }

    public DrawingColor GetDrawingColor()
    {
        var color = this.viewModel.Color;
        return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
    }

    //// --------------------------------------------------------------------------------------------

    private sealed class Vm : ObservableObject
    {
        private readonly List<Color> palette = new();
        private Color color;
        private bool guardReEnter;

        public Vm()
        {
            var config = App.Current.Services.GetRequiredService<IConfiguration>();
            foreach (var data in config.GetSection("ColorPalette").GetChildren())
            {
                if (data.Value is not null)
                {
                    var color = (Color)ColorConverter.ConvertFromString(data.Value);
                    this.palette.Add(color);
                }
            }

            this.SetColorCommand = new RelayCommand<Color>(color => this.Color = color);
        }

        public IList<Color> Palette => this.palette;

        public Color Color
        {
            get => this.color;
            set => this.SetProperty(ref this.color, value);
        }

        public float Alpha
        {
            get => this.color.ScA;
            set
            {
                this.color = Color.FromArgb((byte)(value * 255), this.color.R, this.color.G, this.color.B);
                this.OnPropertyChanged();
            }
        }

        public ICommand SetColorCommand { get; }

        public string ColorKey
        {
            get => this.color.ToString().Substring(startIndex: 1);
            set
            {
                try
                {
                    this.Color = (Color)ColorConverter.ConvertFromString("#" + value);
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                }
            }
        }

        //// --------------------------------------------------------------------------------------------

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (this.guardReEnter)
            {
                return;
            }

            this.guardReEnter = true;

            switch (e.PropertyName)
            {
                case nameof(this.Color):
                    this.OnPropertyChanged(nameof(this.Alpha));
                    this.OnPropertyChanged(nameof(this.ColorKey));
                    break;

                case nameof(this.ColorKey):
                    this.OnPropertyChanged(nameof(this.Alpha));
                    this.OnPropertyChanged(nameof(this.Color));
                    break;

                case nameof(this.Alpha):
                    this.OnPropertyChanged(nameof(this.ColorKey));
                    this.OnPropertyChanged(nameof(this.Color));
                    break;
            }

            this.guardReEnter = false;
        }
    }
}
