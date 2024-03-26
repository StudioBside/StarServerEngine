using System;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Repl;
using GptPlay.Main;

var console = new ReplConsole();

// create config instance from config.json file
if (JsonUtil.TryLoad<GptPlayConfig>("config.json", out var config) == false)
{
    Log.Error($"loading config file failed.");
    return;
}

if (await console.InitializeAsync(new GptPlayHandler(config)) == false)
{
    Log.Error($"Failed to initialize console.");
    return;
}

await console.StartLoop();