namespace CutEditor.Model.Detail;

using System.Collections.Generic;
using System.Drawing;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;

internal static class JsonLoadHelper
{
    public static Color? LoadColor(JToken token, string key)
    {
        var buffer = new List<float>(); // note: 기존 json 포맷은 element가 string으로 되어 있습니다. 
        if (token.TryGetArray(key, buffer, (token, i) => float.Parse(token.ToString())) == false)
        {
            return null;
        }

        if (buffer.Count == 3)
        {
            // 생략된 alpha는 0으로 채움. 
            buffer.Add(0);
        }

        if (buffer.Count != 4)
        {
            return null;
        }

        // 데이터에는 RGBA 순서로 들어있고, 아래 생성자는 ARGB 순서로 받습니다.
        return Color.FromArgb(
            alpha: (byte)(buffer[3] * 255),
            red: (byte)(buffer[0] * 255),
            green: (byte)(buffer[1] * 255),
            blue: (byte)(buffer[2] * 255));
    }
}
