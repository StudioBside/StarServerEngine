namespace Du.Core.Behaviors;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Du.Core.Interfaces;
using Microsoft.Xaml.Behaviors;

public sealed class FrameBehavior : Behavior<Frame>
{
    private static readonly DependencyProperty NavigationSourceProperty;

    /// <summary>
    /// NavigationSource DP 변경 때문에 발생하는 프로퍼티 체인지 이벤트를 막기 위해 사용.
    /// </summary>
    private bool isWork;

    static FrameBehavior()
    {
        NavigationSourceProperty = DependencyProperty.Register(
            name: nameof(NavigationSource),
            propertyType: typeof(string),
            ownerType: typeof(FrameBehavior),
            typeMetadata: new PropertyMetadata(
                defaultValue: null,
                propertyChangedCallback: NavigationSourceChanged));
    }

    public string NavigationSource
    {
        get => (string)this.GetValue(NavigationSourceProperty);
        set => this.SetValue(NavigationSourceProperty, value);
    }

    //// --------------------------------------------------------------------

    protected override void OnAttached()
    {
        this.AssociatedObject.Navigating += this.AssociatedObject_Navigating;
        this.AssociatedObject.Navigated += this.AssociatedObject_Navigated;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.Navigating -= this.AssociatedObject_Navigating;
        this.AssociatedObject.Navigated -= this.AssociatedObject_Navigated;
    }

    private static void NavigationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (FrameBehavior)d;
        if (behavior.isWork)
        {
            return;
        }

        behavior.Navigate();
    }

    private void AssociatedObject_Navigating(object sender, NavigatingCancelEventArgs e)
    {
        //네비게이션 시작전 상황을 뷰모델에 알려주기
        if (this.AssociatedObject.Content is Page pageContent &&
            pageContent.DataContext is INavigationAware navigationAware)
        {
            navigationAware?.OnNavigating(sender, e);
        }
    }

    private void AssociatedObject_Navigated(object sender, NavigationEventArgs e)
    {
        this.isWork = true;
        this.NavigationSource = e.Uri.ToString(); //네비게이션이 완료된 Uri를 NavigationSource에 입력
        this.isWork = false;

        //네비게이션이 완료된 상황을 뷰모델에 알려주기
        if (this.AssociatedObject.Content is Page pageContent &&
            pageContent.DataContext is INavigationAware navigationAware)
        {
            navigationAware.OnNavigated(sender, e);
        }
    }

    private void Navigate()
    {
        switch (this.NavigationSource)
        {
            case "GoBack":
                //GoBack으로 오면 뒤로가기
                if (this.AssociatedObject.CanGoBack)
                {
                    this.AssociatedObject.GoBack();
                }

                break;

            case null:
            case "":
                //아무것도 않함
                return;

            default:
                //navigate
                this.AssociatedObject.Navigate(new Uri(this.NavigationSource, UriKind.RelativeOrAbsolute));
                break;
        }
    }
}
