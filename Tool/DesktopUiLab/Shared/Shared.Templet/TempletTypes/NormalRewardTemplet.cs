namespace Shared.Templet.TempletTypes;

using System;
using System.Text;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Strings;

public enum NormalRewardType
{
    RT_USER_EXP,
    RT_NORMAL_ITEM,
    RT_EQUIP,
    RT_ARTIFACT,
    RT_UNIT,
    RT_ARCANA,
}

public sealed class NormalRewardTemplet
{
    private int quantityMin;
    private int quantityMax;

    public string DebugName => $"[{this.RewardType}/{this.Id}]";
    public NormalRewardType RewardType { get; private set; }
    public int Id { get; private set; }
    public bool HasFixedQuantity => this.quantityMin == this.quantityMax;

    public NormalItem? NormalItemTemplet { get; private set; }
    public Equip? EquipItemTemplet { get; private set; }
    public Unit? UnitTemplet { get; private set; }
    public Arcana? ArcanaTemplet { get; private set; }

    public static NormalRewardTemplet LoadFromJson(JToken token)
    {
        var result = new NormalRewardTemplet();
        result.RewardType = token.GetEnum<NormalRewardType>("RewardType");
        result.Id = token.GetInt32("RewardId");
        result.quantityMin = token.GetInt32("QuantityMin", defValue: 0);
        result.quantityMax = token.GetInt32("QuantityMax", defValue: 0);

        return result;
    }

    public static NormalRewardTemplet LoadFromJson(JToken token, string typeKey, string idKey, string valueKey)
    {
        var result = new NormalRewardTemplet();
        result.RewardType = token.GetEnum<NormalRewardType>(typeKey);
        result.Id = token.GetInt32(idKey);
        result.quantityMin = token.GetInt32(valueKey, defValue: 0);
        result.quantityMax = result.quantityMin;

        return result;
    }

    public void Join()
    {
        switch (this.RewardType)
        {
            case NormalRewardType.RT_NORMAL_ITEM:
                {
                    var templet = TempletContainer<NormalItem>.Find(this.Id);
                    if (templet is null)
                    {
                        ErrorContainer.Add($"{this.DebugName} 아이템 아이디가 잘못 되었습니다.");
                        return;
                    }

                    this.NormalItemTemplet = templet;
                }

                break;

            case NormalRewardType.RT_EQUIP:
                {
                    var templet = TempletContainer<Equip>.Find(this.Id);
                    if (templet is null)
                    {
                        ErrorContainer.Add($"{this.DebugName} 장비 아이디가 잘못 되었습니다.");
                        return;
                    }

                    this.EquipItemTemplet = templet;
                }

                break;

            case NormalRewardType.RT_UNIT:
                {
                    var templet = TempletContainer<Unit>.Find(this.Id);
                    if (templet is null)
                    {
                        ErrorContainer.Add($"{this.DebugName} 유닛 아이디가 잘못 되었습니다.");
                        return;
                    }

                    this.UnitTemplet = templet;
                }

                break;

            case NormalRewardType.RT_ARCANA:
                {
                    var templet = TempletContainer<Arcana>.Find(this.Id);
                    if (templet is null)
                    {
                        ErrorContainer.Add($"{this.DebugName} 아르카나 아이디가 잘못 되었습니다.");
                        return;
                    }

                    this.ArcanaTemplet = templet;
                }

                break;
        }
    }

    public void Validate()
    {
    }
}
