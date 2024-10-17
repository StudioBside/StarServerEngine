namespace Du.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Undo / Redo를 지원하기 위한 단일 커맨드 인터페이스.
/// </summary>
public interface IDormammu
{
    void Undo();
    void Redo();
}
