namespace Shared.Templet.Base;

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public interface ITemplet
{
    public int Key { get; }

    public void Join();
    public void Validate();
}

public interface IGroupTemplet : ITemplet
{
    void LoadGroupData(int groupId, JToken token);
    void Load(JToken token);
}
