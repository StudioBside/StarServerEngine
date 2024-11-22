namespace CutEditor.Model;

using CommunityToolkit.Mvvm.Messaging.Messages;

public static class Messages
{
    public sealed class PreviewChangedMessage(bool showPreview) : ValueChangedMessage<bool>(showPreview);
    public sealed class UpdatePreviewMessage(Cut cut) : ValueChangedMessage<Cut>(cut);
}
