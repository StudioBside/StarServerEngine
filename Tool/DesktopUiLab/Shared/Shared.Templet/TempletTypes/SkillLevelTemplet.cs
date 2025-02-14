namespace Shared.Templet.TempletTypes;

using System;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;
using Shared.Interfaces;
using Shared.Templet.Base;
using Shared.Templet.Errors;
using Shared.Templet.Images;
using Shared.Templet.Strings;

public sealed class SkillLevelTemplet : IGroupTemplet
{
    private readonly List<SkillLevel> levels = new();

    public SkillLevelTemplet()
    {
    }

    public static IEnumerable<SkillLevelTemplet> Values => TempletContainer<SkillLevelTemplet>.Values;
    int ITemplet.Key => this.GroupId;
    public int GroupId { get; private set; }
    public string DebugName => $"[SkillLevelTemplet. Id:{this.GroupId}]";
    public IReadOnlyList<SkillLevel> SkillLevelTemplets => this.levels;

    public void Join()
    {
        foreach (var level in this.levels)
        {
            level.Join();
        }
    }

    public void Validate()
    {
        foreach (var level in this.levels)
        {
            level.Validate();
        }
    }

    public void LoadGroupData(int groupId, JToken token)
    {
        this.GroupId = groupId;
    }

    public void Load(JToken token)
    {
        var level = token.GetInt32("SkillLevelValue");
        var newLevel = new SkillLevel(this.GroupId, level, token);
        this.levels.Add(newLevel);
    }

    public override string ToString()
    {
        return string.Join(", ", this.SkillLevelTemplets.Select(e => e.Name));
    }

    public sealed class SkillLevel
    {
        private readonly string nameStringId;
        private readonly string descStringId;
        private readonly int groupId;
        private readonly int level;

        public SkillLevel(int groupId, int level, JToken token)
        {
            this.groupId = groupId;
            this.level = level;
            this.nameStringId = token.GetString("SkillLevelName");
            this.descStringId = token.GetString("SkillLevelDesc");
        }

        public StringElement? NameElement { get; private set; }
        public StringElement? DescElement { get; private set; }
        public int Level => this.level;
        public string Name => this.NameElement?.Korean ?? this.nameStringId;
        public string Desc => this.DescElement?.Korean ?? this.descStringId;
        public string DebugName => $"[SkillLevelGroup: {this.groupId}] [Lv: {this.level}]";

        public void Join()
        {
            this.NameElement = StringTable.Instance.FindElement(this.nameStringId);
            this.DescElement = StringTable.Instance.FindElement(this.descStringId);
        }

        public void Validate()
        {
            // 몬스터의 경우는 skill level name이 존재하지 않음.
            if (string.IsNullOrEmpty(this.nameStringId) == false && this.nameStringId == this.Name)
            {
                ErrorMessage.Add(ErrorType.Skill, $"{this.DebugName} 의 이름 스트링을 찾을 수 없습니다. [{this.Name}]", this);
            }

            if (this.descStringId == this.Desc)
            {
                ErrorMessage.Add(ErrorType.Skill, $"{this.DebugName} 의 설명 스트링을 찾을 수 없습니다. [{this.Desc}]", this);
            }
        }
    }
}