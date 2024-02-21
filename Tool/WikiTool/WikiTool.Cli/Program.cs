using System;
using Cs.Repl;
using WikiTool.Cli;

var console = new ReplConsole();
console.Initialize(new ReplHandler());
await console.Run();