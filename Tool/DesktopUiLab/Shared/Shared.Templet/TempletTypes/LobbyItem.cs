namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using NKM;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Strings;

public sealed class LobbyItem : ITemplet, ISearchable
{
    private string lobbyNameStringId = string.Empty;

    public LobbyItem(JToken token)
    {
        this.Id = token.GetInt32("SpecialLobbyId");
        this.LobbyType = token.GetEnum("SpecialLobbyType", SpecialLobbyType.SLT_ARCPOINT);
        this.lobbyNameStringId = token.GetString("LobbyName");
        this.ImageFileName = token.GetString("LobbyThumbnail", string.Empty);
    }

    public static string ImageRootPath { get; set; } = string.Empty;
    int ITemplet.Key => this.Id;
    public int Id { get; }
    public SpecialLobbyType LobbyType { get; }
    public string ImageFileName { get; }
    public string ImageFullPath { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    public void Join()
    {
        if (string.IsNullOrEmpty(ImageRootPath))
        {
            Log.ErrorAndExit("LobbyItem.ImageRootPath is not set.");
        }

        if (string.IsNullOrEmpty(this.ImageFileName) == false)
        {
            this.ImageFullPath = Path.Combine(ImageRootPath, this.ImageFileName);
            this.ImageFullPath = Path.GetFullPath(this.ImageFullPath);
        }

        this.Name = StringTable.Instance.Find(this.lobbyNameStringId);
    }

    public void Validate()
    {
    }

    bool ISearchable.IsTarget(string keyword)
    {
        var comparison = StringComparison.CurrentCultureIgnoreCase;
        return this.Id.ToString().Contains(keyword, comparison) ||
               this.Name.Contains(keyword, comparison);
    }
}
