using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class TalkModule
  {
    public string moduleID;
    public string[] stringKeys;
    public bool sequentialStrings;
    [NonSerialized]
    public int sequentialStringLastIndex = -1;
    public bool usesAnimation;
    [ShowInInspectorIf("usesAnimation", false)]
    public string animationName = string.Empty;
    [ShowInInspectorIf("usesAnimation", false)]
    public float animationDuration = -1f;
    public string additionalAnimationName = string.Empty;
    public List<TalkResponse> responses;
    public string noResponseFollowupModule = string.Empty;
    public List<TalkResult> moduleResultActions;

    public void CopyFrom(TalkModule source)
    {
      this.moduleID = source.moduleID + " copy";
      this.stringKeys = new List<string>((IEnumerable<string>) source.stringKeys).ToArray();
      this.sequentialStrings = source.sequentialStrings;
      this.usesAnimation = source.usesAnimation;
      this.animationName = source.animationName;
      this.animationDuration = source.animationDuration;
      this.additionalAnimationName = source.additionalAnimationName;
      this.responses = new List<TalkResponse>((IEnumerable<TalkResponse>) source.responses);
      this.moduleResultActions = new List<TalkResult>((IEnumerable<TalkResult>) source.moduleResultActions);
    }
  }

