namespace Cs.Cli.Detail;

using Cs.Logging;
using IniParser;
using IniParser.Model;
using IniParser.Parser;

internal sealed class HomePathConfig : IHomePathConfig
{
    IEnumerable<string> IHomePathConfig.GetList(HomePathConfigType type)
    {
        yield return $"현재 설정을 확인합니다. 설정의 범위:{type}";

        IniData iniData = this.GetIniData(type);

        int count = 0;
        foreach (var data in iniData.Global)
        {
            yield return $"{data.KeyName}={data.Value}";
            count++;
        }

        foreach (var section in iniData.Sections)
        {
            foreach (var data in section.Keys)
            {
                yield return $"{section.SectionName}.{data.KeyName}={data.Value}";
                count++;
            }
        }

        if (count == 0)
        {
            yield return "설정값이 없습니다.";
        }
    }

    string? IHomePathConfig.GetValue(HomePathConfigType type, string key)
    {
        IniData iniData = this.GetIniData(type);

        if (key.Contains('.'))
        {
            var keys = key.Split('.');
            var section = keys[0];
            var subKey = keys[1];
            return iniData[section][subKey];
        }

        return iniData.Global[key];
    }

    string IHomePathConfig.GetFilePath(HomePathConfigType type) => this.GetFilePath(type);

    void IHomePathConfig.SetValue(HomePathConfigType type, string inputKey, string value)
    {
        if (type == HomePathConfigType.All)
        {
            Log.Warn($"{type} 값이 올바르지 않아 Local 설정 파일을 사용합니다.");
            type = HomePathConfigType.Local;
        }

        var filePath = this.GetFilePath(type);
        Log.Info($"대상 파일:{filePath}");

        var iniData = this.GetIniData(filePath);
        KeyDataCollection targetSection;
        string dataKey;
        if (!inputKey.Contains('.'))
        {
            targetSection = iniData.Global;
            dataKey = inputKey;
        }
        else
        {
            var keys = inputKey.Split('.');
            var section = keys[0];
            dataKey = keys[1];

            targetSection = iniData[section];
        }

        var prevValue = targetSection[dataKey];
        if (prevValue == value)
        {
            Log.Info($"설정값이 변경되지 않았습니다.");
            Log.Info($"  {inputKey} = {value}");
            return;
        }

        targetSection[dataKey] = value;

        Log.Info($"설정값을 변경합니다.");

        if (string.IsNullOrEmpty(prevValue))
        {
            Log.Info($"  {inputKey} = (null) -> {value} (신규 값 추가)");
        }
        else
        {
            Log.Info($"  {inputKey} = {prevValue} -> {value}");
        }

        var parser = new FileIniDataParser();
        parser.WriteFile(filePath, iniData);
    }

    void IHomePathConfig.UnsetValue(HomePathConfigType type, string inputKey)
    {
        if (type == HomePathConfigType.All)
        {
            Log.Warn($"{type} 값이 올바르지 않아 Local 설정 파일을 사용합니다.");
            type = HomePathConfigType.Local;
        }

        var filePath = this.GetFilePath(type);
        Log.Info($"대상 파일:{filePath}");

        var iniData = this.GetIniData(filePath);
        KeyDataCollection targetSection;
        string dataKey;
        if (!inputKey.Contains('.'))
        {
            targetSection = iniData.Global;
            dataKey = inputKey;
        }
        else
        {
            var keys = inputKey.Split('.');
            var section = keys[0];
            dataKey = keys[1];

            targetSection = iniData[section];
        }

        if (targetSection.ContainsKey(dataKey) == false)
        {
            Log.Info($"설정값이 존재하지 않습니다.");
            return;
        }

        var prevValue = targetSection[dataKey];
        targetSection.RemoveKey(dataKey);

        Log.Info($"값을 삭제했습니다.");
        Log.Info($"  {inputKey} = {prevValue}");

        var parser = new FileIniDataParser();
        parser.WriteFile(filePath, iniData);
    }

    void IHomePathConfig.RemoveSection(HomePathConfigType type, string section)
    {
        if (type == HomePathConfigType.All)
        {
            Log.Warn($"{type} 값이 올바르지 않아 Local 설정 파일을 사용합니다.");
            type = HomePathConfigType.Local;
        }

        var filePath = this.GetFilePath(type);
        var iniData = this.GetIniData(filePath);
        Log.Info($"대상 파일:{filePath}");

        if (iniData.Sections.Contains(section) == false)
        {
            Log.Info($"섹션이 존재하지 않습니다.");
            return;
        }

        var sectionData = iniData.Sections.GetSectionData(section);
        iniData.Sections.RemoveSection(section);

        Log.Info($"섹션을 삭제했습니다. 섹션 이름:{section} 데이터 수:{sectionData.Keys.Count}");
        foreach (var data in sectionData.Keys)
        {
            Log.Info($"  {data.KeyName} = {data.Value}");
        }

        var parser = new FileIniDataParser();
        parser.WriteFile(filePath, iniData);
    }

    //// --------------------------------------------------------------

    private IniData GetIniData(HomePathConfigType type)
    {
        if (type == HomePathConfigType.All)
        {
            var iniData = this.GetIniData(this.GetFilePath(HomePathConfigType.Global));
            var localData = this.GetIniData(this.GetFilePath(HomePathConfigType.Local));
            iniData.Merge(localData);
            return iniData;
        }

        return this.GetIniData(this.GetFilePath(type));
    }

    private IniData GetIniData(string filePath)
    {
        var parser = new IniDataParser();

        return File.Exists(filePath)
            ? parser.Parse(File.ReadAllText(filePath))
            : new IniData();
    }

    private string GetFilePath(HomePathConfigType type)
    {
        var executableName = Path.GetFileNameWithoutExtension(Environment.ProcessPath);
        var fileName = $".{executableName}Config";

        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        switch (type)
        {
            case HomePathConfigType.Global:
                return Path.Combine(homeDirectory, fileName);
            case HomePathConfigType.Local:
                return Path.Combine(homeDirectory, $"{fileName}.local");
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
