// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StartBardSong
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Beebyte.Obfuscator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [HutongGames.PlayMaker.Tooltip("Plays a robot bard song.")]
  public class StartBardSong : FsmStateAction
  {
    public bool HasDuration;
    public float Duration = 120f;
    public bool LimitedToFloor = true;
    [CompoundArray("Songs", "Song Type", "Dialogue")]
    public StartBardSong.BardSong[] songsToChooseFrom;
    public FsmString[] songDialogues;
    public FsmString targetDialogueVariable;

    public override void OnEnter()
    {
      PlayerController talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
      int index = Random.Range(0, this.songsToChooseFrom.Length);
      this.ApplySongToPlayer(talkingPlayer, this.songsToChooseFrom[index]);
      this.targetDialogueVariable.Value = this.songDialogues[index].Value;
      this.Finish();
    }

    protected void ApplySongToPlayer(PlayerController targetPlayer, StartBardSong.BardSong targetSong)
    {
      List<StatModifier> activeModifiers = new List<StatModifier>();
      switch (targetSong)
      {
        case StartBardSong.BardSong.DAMAGE_BOOST:
          activeModifiers.Add(new StatModifier()
          {
            statToBoost = PlayerStats.StatType.Damage,
            amount = 1.1f,
            modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE
          });
          break;
        case StartBardSong.BardSong.SPEED_BOOST:
          activeModifiers.Add(new StatModifier()
          {
            statToBoost = PlayerStats.StatType.MovementSpeed,
            amount = 1f,
            modifyType = StatModifier.ModifyMethod.ADDITIVE
          });
          break;
      }
      for (int index = 0; index < activeModifiers.Count; ++index)
        targetPlayer.ownerlessStatModifiers.Add(activeModifiers[index]);
      targetPlayer.stats.RecalculateStats(targetPlayer);
      if (!this.HasDuration && !this.LimitedToFloor)
        return;
      targetPlayer.StartCoroutine(this.HandleSongLifetime(targetPlayer, targetSong, activeModifiers));
    }

    [DebuggerHidden]
    private IEnumerator HandleSongLifetime(
      PlayerController targetPlayer,
      StartBardSong.BardSong targetSong,
      List<StatModifier> activeModifiers)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new StartBardSong__HandleSongLifetimec__Iterator0()
      {
        _this = this
      };
    }

    [Skip]
    public enum BardSong
    {
      DAMAGE_BOOST,
      SPEED_BOOST,
    }
  }
}
