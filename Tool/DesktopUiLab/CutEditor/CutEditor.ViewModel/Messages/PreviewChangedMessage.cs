namespace CutEditor.ViewModel.Messages;

using CommunityToolkit.Mvvm.Messaging.Messages;

public sealed class PreviewChangedMessage(bool showPreview) : ValueChangedMessage<bool>(showPreview);
