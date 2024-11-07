namespace CutEditor.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Dialogs;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Du.Presentation.Util;
using Microsoft.Extensions.DependencyInjection;
using NKM;
using Wpf.Ui;
using Wpf.Ui.Controls;

public sealed class CameraOffsetController : IEnumPicker<CameraOffset>
{
    private readonly Dictionary<CameraOffset, Node> nodes = new();

    public static CameraOffsetController Instance => Singleton<CameraOffsetController>.Instance;
    public IEnumerable<Node> Nodes => this.nodes.Values;
    public Node None { get; private set; } = null!;

    public void Initialize(AssetList assetList)
    {
        var defaultFileName = assetList.CameraOffsetFiles.First(e => e.Contains("default.png"));
        var defaultImage = ImageHelper.Load(defaultFileName);

        foreach (var fileName in assetList.CameraOffsetFiles)
        {
            if (fileName == defaultFileName)
            {
                continue;
            }

            // 전용 이미지가 존재하는 enum 값들을 먼저 생성
            var node = Node.Create(fileName);
            if (node is null)
            {
                Log.Error($"{fileName} is not a valid easing graph file name.");
                continue;
            }

            this.nodes.Add(node.Offset, node);
        }

        // 생성되지 않은 나머지 모든 값들은 default 이미지로 생성
        foreach (var data in EnumUtil<CameraOffset>.GetValues())
        {
            if (this.nodes.ContainsKey(data))
            {
                continue;
            }

            var node = new Node
            {
                Offset = data,
                FileName = defaultFileName,
                ImageSource = defaultImage,
            };
            this.nodes.Add(data, node);

            if (data == CameraOffset.NONE)
            {
                this.None = node;
            }
        }
    }

    public bool TryGetValue(CameraOffset offset, [MaybeNullWhen(false)] out Node node)
    {
        return this.nodes.TryGetValue(offset, out node);
    }

    async Task<IEnumPicker<CameraOffset>.PickResult> IEnumPicker<CameraOffset>.Pick(CameraOffset defaultValue)
    {
        var dialogService = App.Current.Services.GetRequiredService<IContentDialogService>();
        var dialog = new CameraOffsetPickerDialog(defaultValue, dialogService.GetDialogHost());
        var result = await dialog.ShowAsync();
        return result switch
        {
            ContentDialogResult.Primary => new IEnumPicker<CameraOffset>.PickResult(dialog.Result, false),
            ContentDialogResult.Secondary => new IEnumPicker<CameraOffset>.PickResult(CameraOffset.NONE, false),
            _ => new IEnumPicker<CameraOffset>.PickResult(CameraOffset.NONE, true),
        };
    }

    public sealed class Node
    {
        public CameraOffset Offset { get; init; }
        public required string FileName { get; init; }
        public required ImageSource ImageSource { get; init; }

        public static Node? Create(string fileName)
        {
            var nameOnly = Path.GetFileNameWithoutExtension(fileName);
            if (Enum.TryParse(nameOnly, out CameraOffset offset) == false)
            {
                Log.Error($"{fileName} is not a valid CameraOffset enum literal.");
                return null;
            }

            return new Node
            {
                Offset = offset,
                FileName = fileName,
                ImageSource = ImageHelper.Load(fileName),
            };
        }
    }
}