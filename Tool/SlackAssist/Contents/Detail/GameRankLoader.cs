namespace SlackAssist.Contents.Detail;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using GameRank.Core;
using SlackAssist.Configs;
using SlackAssist.Fremawork.Slack;
using SlackNet.Blocks;
using SlackNet.WebApi;

internal class GameRankLoader
{
    public static bool TryLoad([MaybeNullWhen(false)] out Message message)
    {
        message = null;

        var config = SlackAssistConfig.Instance;
        var storage = new FileStorage(config.GameRank.StoragePath);
        var rankData = storage.LoadLatest();
        if (rankData is null)
        {
            return false;
        }

        message = ConvertToSlackMessage(rankData);
        return true;
    }

    private static Message ConvertToSlackMessage(DailyRankData rankData)
    {
        var message = new Message
        {
            IconEmoji = ":googleplay:",
            Username = "GooglePlay",
        };

        using var builder = new MarkdownBuilder();
        builder.Write($"매출 순위 (출처:<{rankData.Source}|{rankData.SourceHost}>. {rankData.Date:yyyy-MM-dd}일 기준.)");
        message.Blocks.Add(builder.FlushToSectionBlock());

        foreach (var game in rankData.Ranks.Take(10))
        {
            builder.WriteLine($"{game.Ranking}. *<{game.SummaryPageUrl}|{game.Title}>*");
            builder.WriteLine($"별점 : {game.StarGrade} [{ToStarMark(game.StarGrade)}]");
            builder.WriteLine($"퍼블리셔 : {game.Publisher}");
            builder.WriteLine($"카테고리 : *<{game.CategoryPageUrl}|{game.CategoryText}>*");

            var section = builder.FlushToSectionBlock();
            section.Accessory = new Image
            {
                ImageUrl = game.ImageUrl.Replace("=s48", "=s96"),
                AltText = game.Title,
            };
            message.Blocks.Add(section);

            //message.Blocks.Add(new DividerBlock());
        }

        return message;

        static string ToStarMark(float value)
        {
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            int fullCount = (int)value;
            for (int i = 0; i < fullCount; i++)
            {
                writer.Write(":star_full:");
            }

            if ((value - fullCount) > 0.01f)
            {
                writer.Write(":star_half:");
            }

            return sb.ToString();
        }
    }
}
