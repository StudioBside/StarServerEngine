﻿namespace Du.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

public interface ICollectionEditor
{
    Task<bool> Edit<T>(IList<T> collection) where T : new();
}
