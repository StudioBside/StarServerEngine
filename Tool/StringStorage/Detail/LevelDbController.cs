namespace StringStorage.Detail;

using System.IO.Compression;
using Cs.Core.Perforce;
using Cs.Core.Util;
using FileLog = Cs.Logging.Log;

internal sealed class LevelDbController : LevelDbReadOnly
{
    private readonly P4Commander p4Commander;
    private bool isDirty;

    public LevelDbController(string path, P4Commander p4Commander) : base(path)
    {
        this.p4Commander = p4Commander;
    }

    public override void Dispose()
    {
        this.Db.Dispose();

        if (!this.isDirty && File.Exists(this.ZipFileName))
        {
            FileLog.Info($"db 변경 사항이 없어 업데이트를 생략합니다. path:{this.Path}");
        }
        else
        {
            // db 업데이트가 발생한 경우 혹은 zip 파일이 없는 경우.
            this.UpdateZipFile();
        }

        // db 폴더를 삭제한다.
        if (Directory.Exists(this.Path))
        {
            Directory.Delete(this.Path, recursive: true);
        }
    }

    public void Upsert(string key, object value)
    {
        this.Db.Put(key, value.ToString());
        this.isDirty = true;
    }

    //// ---------------------------------------------------------------------------------------------

    private void UpdateZipFile()
    {
        // 기존 zip 파일이 존재한다면 백업을 만든다.
        this.TryBackupZipFile();

        ZipFile.CreateFromDirectory(this.Path, this.ZipFileName);
        FileLog.Debug($"db를 압축합니다. zipFile:{this.ZipFileName}");

        if (this.p4Commander.CheckIfRegistered(this.ZipFileName) == false)
        {
            FileLog.Info($"새로운 파일을 등록합니다. file:{this.ZipFileName}");
            this.p4Commander.AddNewFile(this.ZipFileName);
        }
        else if (this.p4Commander.CheckIfOpened(this.ZipFileName) == false)
        {
            FileLog.Info($"파일을 수정합니다. file:{this.ZipFileName}");
            this.p4Commander.OpenForEdit(this.ZipFileName, out _);
        }
        else
        {
            FileLog.Info($"파일이 이미 열려있습니다. file:{this.ZipFileName}");
        }
    }

    private void TryBackupZipFile()
    {
        if (File.Exists(this.ZipFileName) == false)
        {
            return;
        }

        var backupPath = System.IO.Path.ChangeExtension(this.Path, "Backup");
        if (System.IO.Path.Exists(backupPath) == false)
        {
            Directory.CreateDirectory(backupPath);
        }

        var backupFileName = $"{System.IO.Path.GetFileName(this.ZipFileName)}.{ServiceTime.RecentFileString}.bak";
        backupFileName = System.IO.Path.Combine(backupPath, backupFileName);
        File.Move(this.ZipFileName, backupFileName);

        // 백업 파일이 10개 이상이면 가장 오래된 것을 삭제한다.
        var exceptions = new HashSet<string>();
        var backupFiles = Directory.GetFiles(backupPath, "*.bak");
        while (backupFiles.Length > 10)
        {
            var oldest = backupFiles
                .Where(e => exceptions.Contains(e) == false)
                .OrderBy(x => x)
                .First();

            FileLog.Debug($"오래된 백업 파일을 삭제합니다. file:{oldest}");

            try
            {
                File.SetAttributes(oldest, FileAttributes.Normal); // 읽기 전용 속성이 있다면 해제.
                File.Delete(oldest);
            }
            catch (Exception ex)
            {
                FileLog.Error(ex.Message);
                exceptions.Add(oldest);
            }

            backupFiles = Directory.GetFiles(backupPath, "*.bak");
        }
    }
}
