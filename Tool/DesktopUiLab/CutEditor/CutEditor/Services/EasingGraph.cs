namespace CutEditor.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Cs.Core;
using Cs.Logging;
using CutEditor.Dialogs;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Du.Presentation.Util;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Controls;
using static CutEditor.Model.Enums;

public sealed class EasingGraph : IEnumPicker<Ease>
{
    private readonly Dictionary<Ease, Graph> graphs = new();

    public static EasingGraph Instance => Singleton<EasingGraph>.Instance;
    public IEnumerable<Graph> Graphs => this.graphs.Values;

    public void Initialize(AssetList assetList)
    {
        foreach (var fileName in assetList.EasingGraphFiles)
        {
            var graph = Graph.Create(fileName);
            if (graph is null)
            {
                Log.Error($"{fileName} is not a valid easing graph file name.");
                continue;
            }

            this.graphs.Add(graph.Ease, graph);
        }
    }

    public bool TryGetValue(Ease ease, [MaybeNullWhen(false)] out Graph graph)
    {
        return this.graphs.TryGetValue(ease, out graph);
    }

    async Task<IEnumPicker<Ease>.PickResult> IEnumPicker<Ease>.Pick(Ease defaultValue)
    {
        var dialogService = App.Current.Services.GetRequiredService<IContentDialogService>();
        var dialog = new EasingPickerDialog(defaultValue, dialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.Primary => new IEnumPicker<Ease>.PickResult(dialog.Result, false),
            ContentDialogResult.Secondary => new IEnumPicker<Ease>.PickResult(Ease.Unset, false),
            _ => new IEnumPicker<Ease>.PickResult(Ease.Unset, true),
        };
    }

    public sealed class Graph
    {
        public Ease Ease { get; init; }
        public int VisualOrder { get; init; }
        public required string FileName { get; init; }
        public required ImageSource ImageSource { get; init; }

        public static Graph? Create(string fileName)
        {
            var nameOnly = Path.GetFileNameWithoutExtension(fileName);
            if (Enum.TryParse(nameOnly, out Ease ease) == false)
            {
                Log.Error($"{fileName} is not a valid easing graph file name.");
                return null;
            }

            var visualorder = ease switch
            {
                Ease.Unset => 100,
                Ease.Linear => 101,
                _ => (int)ease,
            };

            return new Graph
            {
                Ease = ease,
                VisualOrder = visualorder,
                FileName = fileName,
                ImageSource = ImageHelper.Load(fileName),
            };
        }

        public bool IsInCategory(EaseCategory category)
        {
            return category switch
            {
                EaseCategory.All => true,
                EaseCategory.In => this.Ease.IsIn(),
                EaseCategory.Out => this.Ease.IsOut(),
                EaseCategory.InOut => this.Ease.IsInOut(),
                _ => throw new ArgumentOutOfRangeException(nameof(category)),
            };
        }
    }
}