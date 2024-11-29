namespace Du.Presentation.Extensions;

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.Input;
using Cs.Core;
using Du.Core.Interfaces;

public sealed class PageRouterExtension : MarkupExtension, IPageRouter
{
    private readonly Dictionary<Type/*Param*/, Type/*Page*/> factories = new();
    private Frame frame = null!;

    public PageRouterExtension()
    {
        this.RouteCommand = new RelayCommand<object>(this.Route);
        this.GoToCommand = new RelayCommand<string>(this.OnGoTo);
    }

    public event EventHandler<object>? Navigated;

    public static PageRouterExtension Instance => Singleton<PageRouterExtension>.Instance;
    public ICommand RouteCommand { get; }
    public ICommand GoToCommand { get; }

    public void SetFrame(Frame frame)
    {
        this.frame = frame;
        this.frame.Navigating += this.Frame_Navigating;
        this.frame.Navigated += this.Frame_Navigated;
    }

    public void Route(object? parameter)
    {
        if (parameter is null)
        {
            //Log.Warn($"route error: invalid parameter");
            return;
        }

        var paramType = parameter.GetType();
        if (this.factories.TryGetValue(paramType, out var pageType) == false)
        {
            return;
        }

        this.frame.Navigate(Activator.CreateInstance(pageType, parameter));
    }

    public PageRouterExtension Register<TPage, TParam>() where TPage : Page
    {
        this.factories.Add(typeof(TParam), typeof(TPage));
        return this;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Instance;
    }

    //// ------------------------------------------------------------------------------------

    private void OnGoTo(string? obj)
    {
        if (string.IsNullOrEmpty(obj))
        {
            return;
        }

        this.frame.Navigate(new Uri(obj, UriKind.RelativeOrAbsolute));
    }

    private void Frame_Navigated(object sender, NavigationEventArgs e)
    {
        //네비게이션 시작전 상황을 뷰모델에 알려주기
        if (this.frame.Content is Page pageContent &&
            pageContent.DataContext is INavigationAware navigationAware)
        {
            navigationAware?.OnNavigating(sender, e.Uri);
        }

        this.Navigated?.Invoke(this, e.Content);
    }

    private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
    {
        //네비게이션이 완료된 상황을 뷰모델에 알려주기
        if (this.frame.Content is Page pageContent &&
            pageContent.DataContext is INavigationAware navigationAware)
        {
            navigationAware.OnNavigated(sender, e.Uri);
        }
    }
}
