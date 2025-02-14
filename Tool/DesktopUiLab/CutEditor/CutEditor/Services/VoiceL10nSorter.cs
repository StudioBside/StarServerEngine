namespace CutEditor.Services;

using Cs.Core;
using CutEditor.Model;
using Shared.Templet.Errors;

internal sealed class VoiceL10nSorter
{
    private readonly List<VoiceSet> voiceSets = [];
    private readonly List<VoiceSet> voiceSetsKoreanExists = [];
    public static VoiceL10nSorter Instance => Singleton<VoiceL10nSorter>.Instance;

    public IReadOnlyList<VoiceSet> AllVoiceSets => this.voiceSets;
    public IReadOnlyList<VoiceSet> VoiceSetsKoreanExists => this.voiceSetsKoreanExists;

    public void Initialize(AssetList assetList)
    {
        this.voiceSets.Clear();
        this.voiceSetsKoreanExists.Clear();

        foreach (var v in assetList.VoiceFiles)
        {
            if (VoiceFile.TryCreate(v, out var result, out var language) && result != null && language != null)
            {
                var voiceType = this.voiceSets.FirstOrDefault(e => e.DisplayPath == result.DisplayPath);
                if (voiceType == null)
                {
                    voiceType = new VoiceSet(result.FileName, result.DisplayPath);
                    this.voiceSets.Add(voiceType);
                }

                voiceType.SetData(language.Value, result);
            }
        }

        foreach (var set in this.voiceSets)
        {
            if (set.Korean == null)
            {
                ErrorMessage.Add(ErrorType.Voice, $"{set.DisplayPath} 해당 파일에 매칭되는 한국어 음성이 없습니다. {set.GetDebugStatus()}", this);
            }
            else
            {
                this.voiceSetsKoreanExists.Add(set);
            }
        }
    }
}