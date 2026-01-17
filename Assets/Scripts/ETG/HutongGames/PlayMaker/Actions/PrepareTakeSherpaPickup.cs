// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PrepareTakeSherpaPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class PrepareTakeSherpaPickup : FsmStateAction
  {
    public FsmEvent OnOutOfItems;
    [NonSerialized]
    public int CurrentPickupTargetIndex = -1;
    private SherpaDetectItem m_parentAction;

    public static bool IsOnSherpaMoneyStep
    {
      get
      {
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_READY_FOR_UNLOCKS))
        {
          if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE))
            return GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_ELEMENT1) && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_ELEMENT2);
          if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_COMPLETE))
            return GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_ELEMENT1) && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_ELEMENT2);
          if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_COMPLETE))
            return GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_ELEMENT1) && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_ELEMENT2);
          if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK4_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK4_ELEMENT1))
            return !GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK4_ELEMENT2);
        }
        return false;
      }
    }

    public override void OnEnter()
    {
      if (this.m_parentAction == null)
      {
        for (int index = 0; index < this.Fsm.PreviousActiveState.Actions.Length; ++index)
        {
          if (this.Fsm.PreviousActiveState.Actions[index] is SherpaDetectItem)
          {
            this.m_parentAction = this.Fsm.PreviousActiveState.Actions[index] as SherpaDetectItem;
            break;
          }
        }
      }
      for (int index = 0; index < this.Fsm.ActiveState.Actions.Length; ++index)
      {
        if (this.Fsm.ActiveState.Actions[index] is TakeSherpaPickup)
        {
          (this.Fsm.ActiveState.Actions[index] as TakeSherpaPickup).parentAction = this.m_parentAction;
          break;
        }
      }
      if (this.CurrentPickupTargetIndex >= this.m_parentAction.AllValidTargets.Count)
        this.CurrentPickupTargetIndex = -1;
      ++this.CurrentPickupTargetIndex;
      if (this.CurrentPickupTargetIndex >= this.m_parentAction.AllValidTargets.Count || this.CurrentPickupTargetIndex < 0)
      {
        this.Fsm.Event(this.OnOutOfItems);
        this.Finish();
      }
      else
      {
        PickupObject allValidTarget = this.m_parentAction.AllValidTargets[this.CurrentPickupTargetIndex];
        FsmString fsmString = this.Fsm.Variables.GetFsmString("npcReplacementString");
        EncounterTrackable component = allValidTarget.GetComponent<EncounterTrackable>();
        if (fsmString != null && (UnityEngine.Object) component != (UnityEngine.Object) null)
          fsmString.Value = component.journalData.GetPrimaryDisplayName();
        this.Finish();
      }
    }
  }
}
