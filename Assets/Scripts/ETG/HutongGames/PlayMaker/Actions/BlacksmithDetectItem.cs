// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BlacksmithDetectItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class BlacksmithDetectItem : FsmStateAction
  {
    public DesiredItem[] desires;
    public FsmEvent NextDesireEvent;
    public FsmEvent OutOfDesiresEvent;
    private int m_currentDesireIndex;
    private List<PickupObject> m_currentTargets;
    private int m_currentTargetIndex;
    private bool m_hasNonItemTarget;
    private PlayerController talkingPlayer;
    private PickupObject m_currentTarget;

    public DesiredItem GetCurrentDesire() => this.desires[this.m_currentDesireIndex];

    public PickupObject GetTargetPickupObject() => this.m_currentTarget;

    public override void Reset()
    {
    }

    public override string ErrorCheck() => string.Empty;

    public override void OnEnter()
    {
      this.talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
      this.m_hasNonItemTarget = false;
      this.DoCheck();
      this.Finish();
    }

    private void CheckPlayerForDesire(DesiredItem desire)
    {
      this.m_currentTargets = new List<PickupObject>();
      if (desire.type == DesiredItem.DetectType.SPECIFIC_ITEM)
      {
        PickupObject byId = PickupObjectDatabase.GetById(desire.specificItemId);
        switch (byId)
        {
          case Gun _:
            for (int index = 0; index < this.talkingPlayer.inventory.AllGuns.Count; ++index)
            {
              if (this.talkingPlayer.inventory.AllGuns[index].PickupObjectId == byId.PickupObjectId)
                this.m_currentTargets.Add(byId);
            }
            break;
          case PlayerItem _:
            for (int index = 0; index < this.talkingPlayer.activeItems.Count; ++index)
            {
              if (this.talkingPlayer.activeItems[index].PickupObjectId == byId.PickupObjectId)
                this.m_currentTargets.Add(byId);
            }
            break;
          case PassiveItem _:
            for (int index = 0; index < GameManager.Instance.PrimaryPlayer.passiveItems.Count; ++index)
            {
              if (this.talkingPlayer.passiveItems[index].PickupObjectId == byId.PickupObjectId)
                this.m_currentTargets.Add(byId);
            }
            break;
        }
      }
      else if (desire.type == DesiredItem.DetectType.CURRENCY)
      {
        if (this.talkingPlayer.carriedConsumables.Currency < desire.amount)
          return;
        this.m_hasNonItemTarget = true;
      }
      else if (desire.type == DesiredItem.DetectType.META_CURRENCY)
      {
        if (Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY)) < desire.amount)
          return;
        this.m_hasNonItemTarget = true;
      }
      else
      {
        if (desire.type != DesiredItem.DetectType.KEYS || this.talkingPlayer.carriedConsumables.KeyBullets < desire.amount)
          return;
        this.m_hasNonItemTarget = true;
      }
    }

    private void NextDesire()
    {
      if ((Object) this.m_currentTarget != (Object) null)
        return;
      this.m_currentTargets = (List<PickupObject>) null;
      this.m_currentTargetIndex = 0;
      ++this.m_currentDesireIndex;
      this.Fsm.Event(this.NextDesireEvent);
    }

    private void DoCheck()
    {
      this.m_currentTarget = (PickupObject) null;
      this.m_hasNonItemTarget = false;
      if (this.m_currentDesireIndex >= this.desires.Length)
      {
        this.m_currentDesireIndex = 0;
        this.m_currentTargets = (List<PickupObject>) null;
        this.m_currentTargetIndex = 0;
        this.Fsm.Event(this.OutOfDesiresEvent);
      }
      else
      {
        DesiredItem desire = this.desires[this.m_currentDesireIndex];
        if (GameStatsManager.Instance.GetFlag(desire.flagToSet))
        {
          this.NextDesire();
        }
        else
        {
          if (this.m_currentTargets == null)
          {
            this.m_currentTargetIndex = 0;
            this.CheckPlayerForDesire(desire);
          }
          if (this.m_currentTargetIndex >= this.m_currentTargets.Count && !this.m_hasNonItemTarget)
          {
            this.NextDesire();
          }
          else
          {
            if (this.m_currentTargets.Count > 0)
            {
              PickupObject currentTarget = this.m_currentTargets[this.m_currentTargetIndex];
              this.m_currentTarget = currentTarget;
              ++this.m_currentTargetIndex;
              FsmString fsmString = this.Fsm.Variables.GetFsmString("npcReplacementString");
              EncounterTrackable component = currentTarget.GetComponent<EncounterTrackable>();
              if (fsmString != null && (Object) component != (Object) null)
                fsmString.Value = component.GetModifiedDisplayName();
            }
            DialogueBox dialogueBox = (DialogueBox) null;
            for (int index = 0; index < this.State.Actions.Length; ++index)
            {
              if (this.State.Actions[index] is DialogueBox)
                dialogueBox = this.State.Actions[index] as DialogueBox;
            }
            switch (desire.type)
            {
              case DesiredItem.DetectType.SPECIFIC_ITEM:
                dialogueBox.dialogue[0].Value = "#BLACKSMITH_ASK_FOR_SPECIFIC";
                break;
              case DesiredItem.DetectType.CURRENCY:
                dialogueBox.dialogue[0].Value = "#BLACKSMITH_ASK_FOR_AMOUNT_OF_COINS";
                break;
              case DesiredItem.DetectType.META_CURRENCY:
                dialogueBox.dialogue[0].Value = "#BLACKSMITH_ASK_FOR_AMOUNT_OF_META_CURRENCY";
                break;
              case DesiredItem.DetectType.KEYS:
                dialogueBox.dialogue[0].Value = "#BLACKSMITH_ASK_FOR_AMOUNT_OF_KEYS";
                break;
              default:
                dialogueBox.dialogue[0].Value = "#BLACKSMITH_ASK_FOR_SPECIFIC";
                break;
            }
          }
        }
      }
    }
  }
}
