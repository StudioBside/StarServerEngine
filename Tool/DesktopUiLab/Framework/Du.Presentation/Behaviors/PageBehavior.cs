namespace Du.Presentation.Behaviors;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Du.Core.Interfaces;
using Du.Presentation.Util;
using Microsoft.Xaml.Behaviors;

public class PageBehavior : Behavior<Page>
{
    public static readonly DependencyProperty HandleKeyDownProperty =
          DependencyProperty.Register(
              "HandleKeyDown",
              typeof(bool),
              typeof(PageBehavior),
              new PropertyMetadata(false, OnHandleKeyDownChanged));

    public bool HandleKeyDown
    {
        get => (bool)this.GetValue(HandleKeyDownProperty);
        set => this.SetValue(HandleKeyDownProperty, value);
    }

    protected override void OnAttached()
    {
        this.AssociatedObject.PreviewKeyDown += this.AssociatedObject_PreviewKeyDown;
        this.AssociatedObject.IsVisibleChanged += this.Page_IsVisibleChanged;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewKeyDown -= this.AssociatedObject_PreviewKeyDown;
        this.AssociatedObject.IsVisibleChanged -= this.Page_IsVisibleChanged;
    }

    private static void OnHandleKeyDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private async void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key < Key.A || e.Key > Key.Z)
        {
            return;
        }

        var focusedElement = FocusHelper.GetFocusedElement();
        if (focusedElement is TextBox || focusedElement is PasswordBox)
        {
            return;
        }

        if (this.HandleKeyDown &&
            this.AssociatedObject.DataContext is IKeyDownHandler handler)
        {
            char key = e.Key.ToString().ToLower()[0];
            bool ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            bool alt = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

            e.Handled = await handler.HandleKeyDownAsync(key, ctrl, shift, alt);
        }
    }

    private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // page가 처음 로드된 순간에 키 이벤트를 받지 않기 때문에 아래 처리 추가. Loaded 이벤트로는 해결되지 않음.
        // 참고 : https://stackoverflow.com/a/57648533
        if (this.AssociatedObject.Visibility == Visibility.Visible)
        {
            this.AssociatedObject.Focusable = true;
            this.AssociatedObject.Focus();
        }
    }
}
