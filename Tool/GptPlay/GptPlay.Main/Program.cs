using System;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Repl;
using GptPlay.Main;

var console = new ReplConsole()
{
    Prompt = "번역",
};

// create config instance from config.json file
if (JsonUtil.TryLoad<GptPlayConfig>("config.json", out var config) == false)
{
    Log.Error($"loading config file failed.");
    return;
}

console.Initialize(new InputHandler(console, config));

await console.Run();