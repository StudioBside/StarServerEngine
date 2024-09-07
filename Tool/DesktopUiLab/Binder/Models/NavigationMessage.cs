namespace Binder.Models;

using CommunityToolkit.Mvvm.Messaging.Messages;

public sealed class NavigationMessage(string value) : ValueChangedMessage<string>(value)
{
}
