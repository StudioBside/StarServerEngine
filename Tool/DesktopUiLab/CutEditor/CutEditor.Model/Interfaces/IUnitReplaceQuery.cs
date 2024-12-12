namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.TempletTypes;

public interface IUnitReplaceQuery
{
    Task<Result> QueryAsync(IEnumerable<Cut> cuts);

    public readonly record struct Result(Unit PrevUnit, Unit AfterUnit, bool IsCanceled);
}
