namespace Cs.Repl;

using System;

public sealed class ReplCommandAttribute : Attribute
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}
