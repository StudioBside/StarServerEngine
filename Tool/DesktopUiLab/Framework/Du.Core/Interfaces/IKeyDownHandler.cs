namespace Du.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IKeyDownHandler
{
    bool HandleKeyDown(char key, bool ctrl, bool shift, bool alt);
}
