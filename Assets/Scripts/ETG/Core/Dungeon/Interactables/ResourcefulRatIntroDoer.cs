// Decompiled with JetBrains decompiler
// Type: ResourcefulRatIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class ResourcefulRatIntroDoer : SpecificIntroDoer
  {
    private bool m_isFinished;

    public override Vector2? OverrideIntroPosition
    {
      get => new Vector2?((Vector2) (this.transform.position + new Vector3(39f / 16f, 4f)));
    }

    public override bool IsIntroFinished => this.m_isFinished && base.IsIntroFinished;

    public override void StartIntro(List<tk2dSpriteAnimator> animators)
    {
      base.StartIntro(animators);
      this.StartCoroutine(this.DoIntro());
    }

    [DebuggerHidden]
    public IEnumerator DoIntro()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatIntroDoer__DoIntroc__Iterator0()
      {
        _this = this
      };
    }

    private string SelectIntroString(out bool multiline)
    {
      multiline = false;
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_RESOURCEFULRAT))
        return "#RATFIGHTINTRO_START_POSTVICTORY";
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.HAS_ATTEMPTED_RESOURCEFUL_RAT))
      {
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_COMPLETE) || GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_INTRO_SIX_ALT))
          return "#RATFIGHTINTRO_REPEATED_ATTEMPTS";
        GameStatsManager.Instance.SetFlag(GungeonFlags.RESOURCEFUL_RAT_INTRO_SIX_ALT, true);
        return "#RATFIGHTINTRO_6_NOTES_ATTEMPT_002";
      }
      GameStatsManager.Instance.SetFlag(GungeonFlags.HAS_ATTEMPTED_RESOURCEFUL_RAT, true);
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_06))
      {
        multiline = true;
        return "#RATFIGHTINTRO_6_NOTES_ATTEMPT_001";
      }
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_05))
        return "#RATFIGHTINTRO_5_NOTES";
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_04))
        return "#RATFIGHTINTRO_4_NOTES";
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_01))
      {
        float num = Random.value;
        if ((double) num < 0.33000001311302185)
          return "#RATFIGHTINTRO_3_OR_LESS_NOTES_03";
        return (double) num < 0.6600000262260437 ? "#RATFIGHTINTRO_3_OR_LESS_NOTES_02" : "#RATFIGHTINTRO_3_OR_LESS_NOTES_01";
      }
      multiline = true;
      return "#RATFIGHTINTRO_FOUND_ZERO_NOTES";
    }

    [DebuggerHidden]
    public IEnumerator DoRatTalk(string stringKey, bool multiline)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatIntroDoer__DoRatTalkc__Iterator1()
      {
        multiline = multiline,
        stringKey = stringKey,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator TalkRaw(string plaintext)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatIntroDoer__TalkRawc__Iterator2()
      {
        plaintext = plaintext,
        _this = this
      };
    }
  }

