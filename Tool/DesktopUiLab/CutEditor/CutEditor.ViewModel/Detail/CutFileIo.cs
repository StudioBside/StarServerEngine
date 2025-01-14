namespace CutEditor.ViewModel.Detail;

using System.Text;
using Cs.Core.Perforce;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

internal sealed class CutFileIo
{
    public static string GetTextFileName(string cutsceneName, bool isShorten)
    {
        var fileName = isShorten ? $"CLIENT_SHORTEN_{cutsceneName}.exported" : $"CLIENT_{cutsceneName}.exported";
        return Path.Combine(VmGlobalState.Instance.GetTextFilePath(isShorten), fileName);
    }

    public static IReadOnlyList<Cut> LoadCutData(string cutsceneName, bool isShorten)
    {
        var textFileName = GetTextFileName(cutsceneName, isShorten);
        if (File.Exists(textFileName) == false)
        {
            Log.Warn($"cutscene file not found: {textFileName}");
            return Array.Empty<Cut>();
        }

        var result = new List<Cut>();
        var json = JsonUtil.Load(textFileName);
        json.GetArray("Data", result, (e, i) => new Cut(e, cutsceneName));

        return result;
    }

    public static bool SaveCutData(string cutsceneName, IEnumerable<Cut> cuts, bool isShorten)
    {
        if (P4Commander.TryCreate(out var p4Commander) == false)
        {
            Log.Error($"{cutsceneName} P4Commander 객체 생성 실패");
            return false;
        }

        if (p4Commander.Stream.Contains("/alpha"))
        {
            // 컷 데이터파일은 현재 p4 설정상 임포트(import+) 되어있어서, depot address에는 dev로 표기됩니다.
            p4Commander = p4Commander with { Stream = "//stream/dev" };
        }

        // -------------------------- save text file --------------------------
        var template = StringTemplateFactory.Instance.GetTemplet("CutsOutput", "writeFile");
        if (template is null)
        {
            Log.Error($"{cutsceneName} template not found: CutsOutput.writeFile");
            return false;
        }

        var setting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters =
            [
                new StringEnumConverter(),
            ],
        };

        var rows = cuts.Select(e => e.ToOutputJsonType())
            .Select(e => JsonConvert.SerializeObject(e, setting))
            .ToArray();

        var model = new
        {
            OutputFile = cutsceneName,
            Rows = rows,
        };

        template.Add("model", model);

        var textFilePath = GetTextFileName(cutsceneName, isShorten);
        if (File.Exists(textFilePath))
        {
            File.SetAttributes(textFilePath, FileAttributes.Normal);
        }

        using (var sw = new StreamWriter(textFilePath, append: false, Encoding.UTF8))
        {
            sw.WriteLine(template.Render());
        }

        if (OpenForEdit(p4Commander, textFilePath, "text 파일") == false)
        {
            return false;
        }

        // -------------------------- save binary file --------------------------
        var binFilePath = GetBinFileName(cutsceneName, isShorten);
        if (OutProcess.Run(VmGlobalState.Instance.PackerExeFilePath, $"\"{textFilePath}\" \"{binFilePath}\"", out string result) == false)
        {
            Log.Error($"{cutsceneName} binary 파일 생성 실패.\result:{result}");
            return false;
        }

        if (OpenForEdit(p4Commander, binFilePath, "bin 파일") == false)
        {
            return false;
        }

        var jObject = JsonConvert.DeserializeObject<JObject>(result) ?? throw new Exception("result is not JObject");
        long textFileSize = jObject.GetInt64("TextFileSize");
        //long bsonFileSize = jObject.GetInt64("BsonFileSize");
        long binFileSize = jObject.GetInt64("BinFileSize");
        float downRate = binFileSize * 100f / textFileSize;

        var sb = new StringBuilder();
        sb.AppendLine($"{cutsceneName} 파일을 저장했습니다.");
        sb.AppendLine($"- 텍스트 파일: {textFileSize.ToByteFormat()}");
        sb.AppendLine($"- 바이트 파일: {binFileSize.ToByteFormat()} ({downRate:0.00}%)");
        Log.Info(sb.ToString());
        return true;

        // -- local function
        static bool OpenForEdit(P4Commander p4Commander, string filePath, string name)
        {
            if (p4Commander.CheckIfOpened(filePath) != false)
            {
                return true;
            }

            if (p4Commander.CheckIfChanged(filePath, out bool changed) == false)
            {
                Log.Error($"{name} 변경 여부 확인 실패.\n전체경로:{filePath}");
                return false;
            }

            if (changed == false)
            {
                Log.Info($"{name} 변경사항이 확인되지 않았습니다.\n전체경로:{filePath}");
                return false;
            }

            if (p4Commander.OpenForEdit(filePath, out string p4Output) == false)
            {
                Log.Error($"{name} 오픈 실패.\n전체경로:{filePath}");
                return false;
            }

            return true;
        }
    }

    //// --------------------------------------------------------------------------------------------

    private static string GetBinFileName(string name, bool isShorten)
    {
        var fileName = isShorten ? $"CLIENT_SHORTEN_{name}.bytes" : $"CLIENT_{name}.bytes";
        return Path.Combine(VmGlobalState.Instance.GetBinFilePath(isShorten), fileName);
    }
}
