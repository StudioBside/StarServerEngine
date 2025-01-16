namespace StringStorage.Detail;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using LevelDB;
using FileLog = Cs.Logging.Log;

public class LevelDbReadOnly : IEnumerable<KeyValuePair<string, string>>, IDisposable
{
    private readonly string path;
    private readonly DB db;

    public LevelDbReadOnly(string path)
    {
        this.path = path;
        this.ZipFileName = path + ".zip";

        // zip 파일이 존재하면 항상 새로 압축을 풀어서 db를 준비한다.
        if (File.Exists(this.ZipFileName))
        {
            if (Directory.Exists(path))
            {
                FileLog.Debug($"이전에 사용하던 db 폴더 정리. path:{path}");
                Directory.Delete(path, true);
            }

            ZipFile.ExtractToDirectory(this.ZipFileName, path);
            FileLog.Debug($"db를 새롭게 초기화합니다. zipFile:{this.ZipFileName}");
        }
        else if (Directory.Exists(path))
        {
            // zip 파일은 없는데 로컬에 db 폴더가 있다면 재활용한다.
            FileLog.Debug($"기존 db 폴더를 재활용합니다. path:{path}");
        }
        else
        {
            // zip 파일도 없고 로컬에 db 폴더도 없다면 새로 생성한다.
            FileLog.Debug($"새로운 db 폴더를 생성합니다. path:{path}");
            Directory.CreateDirectory(path);
        }

        var options = new Options { CreateIfMissing = true };
        this.db = new DB(options, path);
    }

    protected string ZipFileName { get; private set; }
    protected DB Db => this.db;
    protected string Path => this.path;

    public void Dispose()
    {
        this.Dispose(disposing: true);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, string>>)this.db).GetEnumerator();
    }

    public bool TryGet<T>(string key, Func<string, T?> factory, [MaybeNullWhen(false)] out T value)
    {
        var valueString = this.db.Get(key);
        if (valueString is null)
        {
            value = default;
            return false;
        }

        var converted = factory(valueString);
        if (converted is null)
        {
            value = default;
            return false;
        }

        value = converted;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this.db).GetEnumerator();
    }

    protected virtual void Dispose(bool disposing)
    {
        this.db.Dispose();

        // db 폴더를 삭제한다.
        if (Directory.Exists(this.path))
        {
            Directory.Delete(this.path, recursive: true);
        }
    }
}