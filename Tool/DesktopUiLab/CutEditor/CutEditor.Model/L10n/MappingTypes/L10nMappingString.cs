namespace CutEditor.Model.L10n.MappingTypes;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Templet.Strings;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

internal sealed class L10nMappingString : ObservableObject, IL10nMapping
{
    private readonly StringElement stringElement;
    private L10nMappingState mappingState;

    public L10nMappingString(string primeKey, StringElement stringElement)
    {
        this.UidStr = primeKey;
        this.stringElement = stringElement;
    }

    public string UidStr { get; }
    public StringElement StringElement => this.stringElement;
    public L10nMappingState MappingState
    {
        get => this.mappingState;
        set => this.SetProperty(ref this.mappingState, value);
    }

    public bool ApplyData(L10nType l10nType)
    {
        return true;
    }
}
