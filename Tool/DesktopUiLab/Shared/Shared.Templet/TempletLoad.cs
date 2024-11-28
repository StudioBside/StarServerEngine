namespace Shared.Templet;

using System;
using Microsoft.Extensions.Configuration;
using Shared.Templet.Base;
using Shared.Templet.Strings;
using Shared.Templet.TempletTypes;

public static class TempletLoad
{
    public static void Execute(IConfiguration config)
    {
        Load(config);
        Join();
        Validate();
    }

    //// --------------------------------------------------------------------

    private static void Load(IConfiguration config)
    {
        var templetRoot = config["TempletDataRoot"] ?? throw new Exception("TempletDataRoot is not set in the configuration file.");
        var unitSmallImageRoot = config["UnitSmallImageRoot"] ?? throw new Exception("UnitSmallImageRoot is not set in the configuration file.");
        var unitImageRoot = config["UnitImageRoot"] ?? throw new Exception("UnitImageRoot is not set in the configuration file.");
        var bgImageRoot = config["BgImageRoot"] ?? throw new Exception("BgImageRoot is not set in the configuration file.");
        var lobbyThumbnailRoot = config["LobbyThumbnailRoot"] ?? throw new Exception("LobbyThumbnailRoot is not set in the configuration file.");

        TempletLoader.TempletRootPath = templetRoot;
        Unit.SmallImageRootPath = unitSmallImageRoot;
        Unit.ImageRootPath = unitImageRoot;
        LobbyItem.ImageRootPath = lobbyThumbnailRoot;

        var fullPath = Path.Combine(templetRoot, "CLIENT_STRING_KOR.exported");
        StringTable.Instance.Load(fullPath);

        string[] unitFileNames =
        [
            "CLIENT_UNIT_TEMPLET_BASE1.exported",
            "CLIENT_UNIT_TEMPLET_BASE2.exported",
            "CLIENT_UNIT_TEMPLET_BASE3.exported",
            "CLIENT_UNIT_TEMPLET_BASE4.exported",
        ];
        TempletLoader.BuildContainer(unitFileNames, e => new Unit(e), e => e.StrId);
        TempletLoader.BuildContainer("CLIENT_LOBBY_ITEM_TEMPLET.exported", e => new LobbyItem(e));
    }

    private static void Join()
    {
        TempletContainerUtil.InvokeJoin();
    }

    private static void Validate()
    {
        TempletContainerUtil.InvokeValidate();
    }
}
