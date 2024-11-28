namespace CutEditor.Services;

using CutEditor.Model;
using Du.Presentation.Util;
using Shared.Templet.TempletTypes;

public static class ThumbnailMaker
{
    public static void UpdateAll()
    {
        foreach (var data in AssetList.Instance.BgImageFiles)
        {
            ImageHelper.CreateThumbnail(data, thumbnailWidth: 200);
        }

        foreach (var data in AssetList.Instance.StoryImageFiles)
        {
            ImageHelper.CreateThumbnail(data, thumbnailWidth: 200);
        }

        foreach (var data in Unit.Values)
        {
            if (string.IsNullOrEmpty(data.IllustPath) == false)
            {
                ImageHelper.CreateThumbnail(data.IllustPath, thumbnailWidth: 200);
            }

            if (string.IsNullOrEmpty(data.IllustUiPath) == false)
            {
                ImageHelper.CreateThumbnail(data.IllustUiPath, thumbnailWidth: 200);
            }
        }
    }
}
