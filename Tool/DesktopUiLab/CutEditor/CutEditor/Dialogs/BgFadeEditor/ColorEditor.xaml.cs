namespace CutEditor.Dialogs.BgFadeEditor;

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
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
        var color = Color.FromArgb(modelColor.A, modelColor.R, modelColor.G, modelColor.B);
        this.viewModel.SetColor(color);
        this.IsEnabled = isEnabled;
    }

    public DrawingColor GetDrawingColor()
    {
        var color = this.viewModel.GetColor();
        return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
    }

    //// --------------------------------------------------------------------------------------------

    private sealed class Vm : ObservableObject
    {
        private Color rgb;
        private float alpha;
        private bool guardReEnter;

        public Color Rgb => this.rgb;

        public float Alpha
        {
            get => this.alpha;
            set => this.SetProperty(ref this.alpha, value);
        }

        public string ColorKey
        {
            get => this.GetColor().ToString().Substring(startIndex: 1);
            set => this.SetColor((Color)ColorConverter.ConvertFromString("#" + value));
        }

        public void SetColor(Color color)
        {
            this.rgb = color;
            color.A = 255;

            this.alpha = color.A / 255f;
            this.OnPropertyChanged(nameof(this.ColorKey));
        }

        public Color GetColor()
        {
            return new Color
            {
                R = this.rgb.R,
                G = this.rgb.G,
                B = this.rgb.B,
                A = (byte)(this.alpha * 255),
            };
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
                case nameof(this.ColorKey):
                    this.OnPropertyChanged(nameof(this.Alpha));
                    this.OnPropertyChanged(nameof(this.Rgb));
                    break;

                case nameof(this.Alpha):
                    this.OnPropertyChanged(nameof(this.ColorKey));
                    this.OnPropertyChanged(nameof(this.Rgb));
                    break;
            }

            this.guardReEnter = false;
        }
    }
}
