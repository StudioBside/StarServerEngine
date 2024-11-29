namespace Du.Presentation.Extensions;

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
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
    }

    public static PageRouterExtension Instance => Singleton<PageRouterExtension>.Instance;
    public ICommand RouteCommand { get; }

    public void SetFrame(Frame frame)
    {
        this.frame = frame;
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
}
