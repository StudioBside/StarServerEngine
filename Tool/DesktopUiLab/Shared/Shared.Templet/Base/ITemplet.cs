namespace Shared.Templet.Base;

using System;
using System.Collections.Generic;

public interface ITemplet
{
    public int Key { get; }

    public void Join();
    public void Validate();
}
