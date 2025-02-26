namespace Cs.Cli.Authentication;

using System.Text;
using Cs.Cli.Authentication.KeyStores;
using Cs.Core.Util;

internal sealed class Authenticator : IAuthenticator
{
    private const string TheAnswer = "7f70a8f6c4d610fc4f667e6811c4487a";

    private static readonly IKeyStore KeyStore;
    private static readonly string PasswordKeyName;

    static Authenticator()
    {
        PasswordKeyName = $"{Path.GetFileNameWithoutExtension(Environment.ProcessPath)}Auth";
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            KeyStore = new KeyStoreDefault(PasswordKeyName);
        }
    }

    public bool CheckAuthorizedUser()
    {
        if (HasAuthorizedFlag())
        {
            return true;
        }

        // prompt user to enter the key
        Console.WriteLine("비밀번호 확인이 필요합니다:");
        string password = ReadPassword();
        var checksum = password.CalcMd5Checksum();
        if (checksum.Equals(TheAnswer, StringComparison.CurrentCultureIgnoreCase) == false)
        {
            Console.WriteLine($"비밀번호가 올바르지 않습니다. checksum:{checksum}");
            return false;
        }

        KeyStore.WritePassword(PasswordKeyName, checksum);
        return true;
    }

    //// --------------------------------------------------------------------------------

    private static bool HasAuthorizedFlag()
    {
        return KeyStore.TryGetPassword(PasswordKeyName, out var password) &&
            password.Equals(TheAnswer, StringComparison.CurrentCultureIgnoreCase);
    }

    private static string ReadPassword()
    {
        var password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true); // 입력된 키를 화면에 표시하지 않음

            if (key.Key == ConsoleKey.Enter) // 엔터 입력 시 종료
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0) // 백스페이스 처리
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b"); // 콘솔에서 문자 삭제 효과
            }
            else if (!char.IsControl(key.KeyChar)) // 일반 문자 입력
            {
                password.Append(key.KeyChar);
                Console.Write("*"); // 입력된 문자를 '*'로 표시
            }
        }

        return password.ToString();
    }
}
