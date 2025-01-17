namespace StarCli.Commands;

using System;
using System.CommandLine;
using Cs.Cli;
using Cs.Core.Util;
using Cs.Logging;

internal sealed class ConfigCommand : CommandBase
{
    public ConfigCommand(IHomePathConfig config) : base(config, "config", "설정값 관리")
    {
        var argKey = new Argument<string>("key", "설정 키를 지정합니다");
        var argValue = new Argument<string>("value", "설정 값을 지정합니다");

        var optGlobal = new Option<bool>(["--global", "-g"], "전역 설정을 조회합니다");
        var optLocal = new Option<bool>(["--local", "-l"], "로컬 설정을 조회합니다");
        this.AddGlobalOption(optGlobal);
        this.AddGlobalOption(optLocal);

        // 읽기 작업 설정은 All 타입이 기본 옵션입니다. -----------------------------------------------------
        {
            var subCommand = new Command("list", "현재 설정을 모두 나열합니다") { optGlobal, optLocal };
            subCommand.SetHandler(this.OnRecvList, optGlobal, optLocal);
            this.AddCommand(subCommand);

            subCommand = new Command("get", "설정값을 조회합니다") { optGlobal, optLocal, argKey };
            subCommand.SetHandler(this.OnRecvGet, optGlobal, optLocal, argKey);
            this.AddCommand(subCommand);
        }

        // 쓰기 작업 설정은 Local 타입이 기본 옵션입니다. -----------------------------------------------------
        {
            var subCommand = new Command("set", "설정값을 변경합니다") { optGlobal, optLocal, argKey, argValue };
            var handleSet = (bool global, bool local, string key, string value) =>
            {
                var configType = ToConfigType(global, local, HomePathConfigType.Local);
                this.Config.SetValue(configType, key, value);
            };
            subCommand.SetHandler(handleSet, optGlobal, optLocal, argKey, argValue);
            this.AddCommand(subCommand);

            subCommand = new Command("unset", "설정값을 삭제합니다") { optGlobal, optLocal, argKey };
            var handleUnset = (bool global, bool local, string key) =>
            {
                var configType = ToConfigType(global, local, HomePathConfigType.Local);
                this.Config.UnsetValue(configType, key);
            };
            subCommand.SetHandler(handleUnset, optGlobal, optLocal, argKey);
            this.AddCommand(subCommand);

            var argSection = new Argument<string>("section", "삭제할 섹션의 이름");
            subCommand = new Command("remove-section", "섹션을 삭제합니다") { optGlobal, optLocal, argSection };
            var handleRemoveSection = (bool global, bool local, string section) =>
            {
                var configType = ToConfigType(global, local, HomePathConfigType.Local);
                this.Config.RemoveSection(configType, section);
            };
            subCommand.SetHandler(handleRemoveSection, optGlobal, optLocal, argSection);
            this.AddCommand(subCommand);

            subCommand = new Command("edit", "설정 파일을 기본 편집기에서 열어줍니다") { optGlobal, optLocal };
            subCommand.SetHandler(this.OnRecvEdit, optGlobal, optLocal);
            this.AddCommand(subCommand);
        }
    }

    private static HomePathConfigType ToConfigType(bool global, bool local, HomePathConfigType defaultValue)
    {
        if (global == false && local == false)
        {
            return defaultValue;
        }

        return global ? HomePathConfigType.Global : HomePathConfigType.Local;
    }

    private void OnRecvEdit(bool global, bool local)
    {
        var configType = ToConfigType(global, local, HomePathConfigType.Local);
        var path = this.Config.GetFilePath(configType);
        Log.Info($"open {path} ...");

        var process = OutProcess.OpenDefaultSetting(path);
        if (process is null)
        {
            Log.Error($"설정 파일을 열 수 없습니다.");
            return;
        }

        process.WaitForExit();
    }

    private void OnRecvGet(bool global, bool local, string key)
    {
        var configType = ToConfigType(global, local, HomePathConfigType.All);
        var value = this.Config.GetValue(configType, key);
        if (value is null)
        {
            Log.Info("설정값이 없습니다.");
        }
        else
        {
            Log.Info($"{key} = {value}");
        }
    }

    private void OnRecvList(bool global, bool local)
    {
        var configType = ToConfigType(global, local, HomePathConfigType.All);
        foreach (var data in this.Config.GetList(configType))
        {
            Log.Info(data);
        }
    }
}
