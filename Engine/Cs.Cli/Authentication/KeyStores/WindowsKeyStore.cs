namespace Cs.Cli.Authentication.KeyStores;

using System.Diagnostics.CodeAnalysis;

internal sealed class WindowsKeyStore : IKeyStore
{
    public bool TryGetPassword(string key, [MaybeNullWhen(false)] out string password)
    {
        throw new NotImplementedException();
    }

    public void WritePassword(string key, string password)
    {
        throw new NotImplementedException();
    }
}
