namespace Du.WpfLib;

using System;
using System.Windows;
using System.Windows.Input;

public partial class StringInputDialog : Window
{
    public StringInputDialog(string message, string defaultValue)
    {
        this.Message = message;
        this.UserInput = defaultValue;

        this.Owner = Application.Current.MainWindow;
        this.DataContext = this;
        this.InitializeComponent();
    }

    public string Message { get; }
    public string UserInput { get; set; } = string.Empty;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = true; // 창을 닫으면서 결과 반환
        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.CancelButton_Click(sender, e); // ESC 키가 눌렸을 때 창을 닫음
        }

        if (e.Key == Key.Enter)
        {
            this.OkButton_Click(sender, e); // Enter 키가 눌렸을 때 OK 버튼 클릭
        }
    }
}
