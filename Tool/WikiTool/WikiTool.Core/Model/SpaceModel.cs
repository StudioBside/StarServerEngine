namespace WikiTool.Core;

public sealed class SpaceModel
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "global" or "personal"
}

/*
{
    "id": 24281093,
    "key": "QkL2rjCmJyjr",
    "name": "스타 테크",
    "type": "global",
    "status": "current",
    "_expandable": {
        "settings": "/rest/api/space/QkL2rjCmJyjr/settings",
        "metadata": "",
        "operations": "",
        "lookAndFeel": "/rest/api/settings/lookandfeel?spaceKey=QkL2rjCmJyjr",
        "identifiers": "",
        "permissions": "",
        "icon": "",
        "description": "",
        "theme": "/rest/api/space/QkL2rjCmJyjr/theme",
        "history": "",
        "homepage": "/rest/api/content/24281301"
    },
    "_links": {
        "webui": "/spaces/QkL2rjCmJyjr",
        "self": "https://starsavior.atlassian.net/wiki/rest/api/space/QkL2rjCmJyjr"
    }
},
*/