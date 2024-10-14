namespace Du.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IClipboardHandler
{
    bool HandlePastedText(string text);
}
