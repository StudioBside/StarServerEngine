namespace Cs.HttpServer;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cs.Core.Util;

public sealed class UrlAclList
{
    private readonly HashSet<string> values = new HashSet<string>();

    public UrlAclList()
    {
        this.Initialize();
    }

    public IEnumerable<string> Values => this.values;

    public bool Contains(string data)
    {
        return this.values.Contains(data);
    }

    private void Initialize()
    {
        if (OutProcess.Run("netsh", "http show urlacl", out var output) == false)
        {
            throw new Exception($"[UrlAclList] get urlacl list failed.");
        }

        foreach (Match? match in Regex.Matches(output, @"http[^\s]+", RegexOptions.Multiline))
        {
            if (match != null)
            {
                this.values.Add(match.Value);
            }
        }
    }
}
