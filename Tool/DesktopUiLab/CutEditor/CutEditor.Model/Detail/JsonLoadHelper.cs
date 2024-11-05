namespace CutEditor.Model.Detail;

using System.Collections.Generic;
using System.Drawing;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;

internal static class JsonLoadHelper
{
    public static Color? LoadColor(JToken token, string key)
    {
        var buffer = new List<int>();
        if (token.TryGetArray(key, buffer) == false)
        {
            return null;
        }

        if (buffer.Count != 4)
        {
            return null;
        }

        // 데이터에는 RGBA 순서로 들어있고, 아래 생성자는 ARGB 순서로 받습니다.
        return Color.FromArgb(buffer[3], buffer[0], buffer[1], buffer[2]);
    }
}
