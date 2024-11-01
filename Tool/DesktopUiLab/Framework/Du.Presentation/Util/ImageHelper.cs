namespace Du.Presentation.Util;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cs.Core.Util;

public static class ImageHelper
{
    private const string ThumbnailRoot = "./ThumbnailCache";

    /// <summary>
    /// PNG 파일을 로드하여 비율을 유지한 썸네일 이미지를 생성하고 저장한 후, 화면에 출력할 수 있는 ImageSource를 반환합니다.
    /// 이미 썸네일이 존재하면 새로 생성하지 않고 바로 로드합니다.
    /// </summary>
    /// <param name="filePath">원본 PNG 파일 경로.</param>
    /// <param name="thumbnailWidth">썸네일의 가로 사이즈.</param>
    /// <returns>썸네일 이미지의 ImageSource.</returns>
    public static ImageSource CreateThumbnail(string filePath, int thumbnailWidth)
    {
        // 썸네일 파일 경로 생성
        string thumbnailPath = Path.Combine(ThumbnailRoot, $"{Path.GetFileNameWithoutExtension(filePath)}_{thumbnailWidth}.png");
        FileSystem.GuaranteePath(thumbnailPath);

        // 썸네일이 이미 존재하는 경우, 로드하여 반환
        if (File.Exists(thumbnailPath))
        {
            var existingThumbnail = new BitmapImage();
            using (var stream = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read))
            {
                existingThumbnail.BeginInit();
                existingThumbnail.CacheOption = BitmapCacheOption.OnLoad;
                existingThumbnail.StreamSource = stream;
                existingThumbnail.EndInit();
            }

            return existingThumbnail;
        }

        // 원본 이미지 로드
        var bitmap = new BitmapImage();
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
        }

        // 원본 이미지 비율 계산
        double aspectRatio = (double)bitmap.PixelHeight / bitmap.PixelWidth;
        int thumbnailHeight = (int)(thumbnailWidth * aspectRatio);

        // 썸네일 생성
        var thumbnail = new TransformedBitmap(bitmap, new ScaleTransform(
            (double)thumbnailWidth / bitmap.PixelWidth,
            (double)thumbnailHeight / bitmap.PixelHeight));

        // 썸네일을 파일에 저장
        using (var fileStream = new FileStream(thumbnailPath, FileMode.Create))
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(thumbnail));
            encoder.Save(fileStream);
        }

        // 썸네일의 ImageSource 반환
        return thumbnail;
    }
}
