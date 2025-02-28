namespace Shared.Templet.TempletTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Templet.Base;

public sealed class NormalRewardGroupTemplet : IGroupTemplet
{
    private readonly List<CaseData> lottery = new();
    public int GroupId { get; private set; }
    public int TotalRatio { get; private set; }
    public int Key => this.GroupId;
    public string ContentsTag { get; private set; } = string.Empty;
    public string DebugName => $"[{this.GroupId}]";
    public IEnumerable<NormalRewardTemplet> RewardTemplets => this.lottery.Select(e => e.Value);

    public static NormalRewardGroupTemplet? Find(int key) => TempletContainer<NormalRewardGroupTemplet>.Find(key);
    public static bool TryGet(int key, out NormalRewardGroupTemplet? result) => TempletContainer<NormalRewardGroupTemplet>.TryGet(key, out result);

    public void LoadGroupData(int groupId, JToken token)
    {
        this.GroupId = groupId;
        this.ContentsTag = token.GetString("ContentsTag");
    }

    public void Load(JToken token)
    {
        var templet = NormalRewardTemplet.LoadFromJson(token);

        int ratio = token.GetInt32("Ratio");
        this.TotalRatio += ratio;
        this.lottery.Add(new CaseData(ratio, this.TotalRatio, templet));
    }

    public void Join()
    {
        foreach (var data in this.lottery)
        {
            data.Value.Join();
        }
    }

    public void Validate()
    {
        foreach (var data in this.lottery)
        {
            data.Value.Validate();
        }
    }

    public List<(NormalRewardTemplet Reward, float Value)> GetRatio()
    {
        var result = new List<(NormalRewardTemplet, float)>();
        foreach (var data in this.lottery)
        {
            result.Add((data.Value, (float)data.Ratio / this.TotalRatio));
        }

        return result;
    }

    public readonly struct CaseData
    {
        public CaseData(int ratio, int accumulatedRatio, NormalRewardTemplet value)
        {
            this.Ratio = ratio;
            this.AccumulatedRatio = accumulatedRatio;
            this.Value = value;
        }

        public int Ratio { get; }
        public int AccumulatedRatio { get; }
        public NormalRewardTemplet Value { get; }
    }
}
