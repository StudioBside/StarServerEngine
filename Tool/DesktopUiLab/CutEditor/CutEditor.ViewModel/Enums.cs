namespace CutEditor.ViewModel;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Enums
{
    public enum CutDataType
    {
        [Description("일반형 (Ctrl+1)")]
        Normal,
        [Description("브랜치컷 (Ctrl+2)")]
        Branch,
    }
}
