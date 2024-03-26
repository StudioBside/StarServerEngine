using System;
using Cs.Repl;
using WikiTool.Cli;
using WikiTool.Core;

var handler = new WikiToolHandler();
var console = new ReplConsole();
if (await console.InitializeAsync(handler) == false)
{
    return;
}

if (handler.Config.BatchCommands.Count > 0)
{
    await console.ExecuteBatch(handler.Config.BatchCommands);
}

await console.StartLoop();