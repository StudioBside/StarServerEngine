namespace Du.Core;

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Cs.Core.Util;

public static class Messages
{
    public sealed class NavigationMessage(string value) : ValueChangedMessage<string>(value);
    public sealed class DataChangedMessage([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        public string Tag => TagBuilder.Build(file, line);
    }
}
