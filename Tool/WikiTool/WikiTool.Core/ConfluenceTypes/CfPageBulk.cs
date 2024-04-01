namespace WikiTool.Core.ConfluenceTypes;

using Cs.Core.Util;
using Newtonsoft.Json.Linq;

public sealed class CfPageBulk
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required int ParentId { get; set; }
    public required PageBody Body { get; set; }
    public required string Status { get; set; }
    public required PageVersion Version { get; set; }
    public int VersionNumber => this.Version.Number;
    
    public static CfPageBulk LoadFromJson(JToken obj, int index)
    {
        PageBody pageBody = null!;
        if (obj["body"] is { } body && body["storage"] is { } storage)
        {
            pageBody = storage.ToObject<PageBody>()!;
        }

        return new CfPageBulk
        {
            Id = obj.GetInt32("id"),
            Title = obj.GetString("title"),
            ParentId = obj.GetInt32("parentId", 0),
            Body = pageBody,
            Status = obj.GetString("status"),
            Version = obj["version"]!.ToObject<PageVersion>()!,
        };
    }

    internal void Update(CfPageBulk bulkPage)
    {
        // update members
        this.Title = bulkPage.Title;
        this.ParentId = bulkPage.ParentId;
        this.Body = bulkPage.Body;
        this.Status = bulkPage.Status;
        this.Version = bulkPage.Version;
    }

    public sealed class PageVersion
    {
        public int Number { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool MinorEdit { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    
    public sealed class PageBody
    {
        public string Representation { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}

/*
        {
            "parentType": null,
            "createdAt": "2024-02-01T02:34:14.885Z",
            "authorId": "70121:837c953e-d704-419c-a5db-e1a80bb4134e",
            "id": "24281301",
            "version": {
                "number": 1,
                "message": "",
                "minorEdit": false,
                "authorId": "70121:837c953e-d704-419c-a5db-e1a80bb4134e",
                "createdAt": "2024-02-01T02:34:18.865Z"
            },
            "title": "스타 테크",
            "status": "current",
            "ownerId": "70121:837c953e-d704-419c-a5db-e1a80bb4134e",
            "body": {},
            "parentId": null,
            "spaceId": "24281093",
            "lastOwnerId": null,
            "position": 854,
            "_links": {
                "editui": "/pages/resumedraft.action?draftId=24281301",
                "webui": "/spaces/QkL2rjCmJyjr/overview",
                "tinyui": "/x/1YByAQ"
            }
        },
*/