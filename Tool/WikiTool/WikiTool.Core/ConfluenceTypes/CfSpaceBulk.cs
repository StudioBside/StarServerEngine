namespace WikiTool.Core.ConfluenceTypes;

public sealed class CfSpaceBulk
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "global" or "personal"
}

/*
{
  "results": [
        {
            "createdAt": "2024-02-01T02:34:13.928Z",
            "authorId": "70121:837c953e-d704-419c-a5db-e1a80bb4134e",
            "homepageId": "24281301",
            "icon": null,
            "name": "스타 테크",
            "key": "QkL2rjCmJyjr",
            "id": "24281093",
            "type": "global",
            "description": null,
            "status": "current",
            "_links": {
                "webui": "/spaces/QkL2rjCmJyjr"
            }
        }
  ],
  "_links": {
    "next": "<string>",
    "base": "<string>"
  }
}
*/