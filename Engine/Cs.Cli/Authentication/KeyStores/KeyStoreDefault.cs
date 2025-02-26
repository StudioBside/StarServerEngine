namespace Cs.Cli.Authentication.KeyStores;

using System.Diagnostics.CodeAnalysis;

internal sealed class KeyStoreDefault : IKeyStore
{
    private readonly string ticketFilePath;

    public KeyStoreDefault(string passwordKeyName)
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var fileName = $"{passwordKeyName}.txt";
        this.ticketFilePath = Path.Join(homeDirectory, fileName);
    }

    public bool TryGetPassword(string key, [MaybeNullWhen(false)] out string password)
    {
        if (File.Exists(this.ticketFilePath) == false)
        {
            password = null;
            return false;
        }

        password = File.ReadAllText(this.ticketFilePath);
        return string.IsNullOrEmpty(password);
    }

    public void WritePassword(string key, string password)
    {
        File.WriteAllText(this.ticketFilePath, password);
    }
}
