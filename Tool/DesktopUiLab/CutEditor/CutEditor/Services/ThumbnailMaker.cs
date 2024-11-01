namespace CutEditor.Services;

using CutEditor.Model;
using Du.Presentation.Util;

public static class ThumbnailMaker
{
    public static void UpdateAll()
    {
        foreach (var data in AssetList.Instance.BgImageFiles)
        {
            ImageHelper.CreateThumbnail(data, thumbnailWidth: 100);
        }

        foreach (var data in AssetList.Instance.StoryImageFiles)
        {
            ImageHelper.CreateThumbnail(data, thumbnailWidth: 100);
        }
    }
}
