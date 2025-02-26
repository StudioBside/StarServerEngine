namespace Cs.Cli.Authentication;

using System.Diagnostics.CodeAnalysis;

internal interface IKeyStore
{
    bool TryGetPassword(string key, [MaybeNullWhen(false)] out string password);
    void WritePassword(string key, string password);
}
