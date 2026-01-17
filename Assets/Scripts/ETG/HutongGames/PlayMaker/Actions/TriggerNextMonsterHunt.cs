// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TriggerNextMonsterHunt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  public class TriggerNextMonsterHunt : FsmStateAction
  {
    public FsmBool OnlySetText = (FsmBool) false;
    private TalkDoerLite m_talkDoer;

    public override void Reset()
    {
    }

    public override string ErrorCheck() => string.Empty;

    public override void OnEnter()
    {
      this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
      if (!this.OnlySetText.Value)
      {
        int amountToDrop = GameStatsManager.Instance.huntProgress.TriggerNextQuest();
        if (amountToDrop > 0)
          LootEngine.SpawnCurrency(this.m_talkDoer.sprite.WorldCenter, amountToDrop, true, new Vector2?(Vector2.down * 1.75f), new float?(45f));
      }
      FsmString fsmString1 = this.Fsm.Variables.GetFsmString("QuestIntroString");
      if (fsmString1 != null && GameStatsManager.Instance.huntProgress.ActiveQuest != null)
      {
        fsmString1.Value = GameStatsManager.Instance.huntProgress.ActiveQuest.QuestIntroString;
        DialogueBox.DialogueSequence dialogueSequence = DialogueBox.DialogueSequence.Mutliline;
        if (fsmString1.Value.Contains("_GENERIC"))
          dialogueSequence = DialogueBox.DialogueSequence.Default;
        if (this.State.Transitions.Length > 0)
        {
          FsmState state1 = this.Fsm.GetState(this.State.Transitions[0].ToState);
          for (int index = 0; index < state1.Actions.Length; ++index)
          {
            if (state1.Actions[index] is DialogueBox)
              (state1.Actions[index] as DialogueBox).sequence = dialogueSequence;
            FsmState state2 = this.Fsm.GetState(state1.Transitions[0].ToState);
            if (state2.Actions[0] is DialogueBox)
              state2.Actions[0].Enabled = dialogueSequence == DialogueBox.DialogueSequence.Default;
          }
        }
      }
      if (!string.IsNullOrEmpty(GameStatsManager.Instance.huntProgress.ActiveQuest.TargetStringKey))
      {
        Debug.Log((object) "doing 1");
        FsmString fsmString2 = this.Fsm.Variables.GetFsmString("npcReplacementString");
        if (fsmString2 != null)
        {
          Debug.Log((object) ("doing 2: " + GameStatsManager.Instance.huntProgress.GetReplacementString()));
          fsmString2.Value = GameStatsManager.Instance.huntProgress.GetReplacementString();
        }
      }
      FsmInt fsmInt = this.Fsm.Variables.GetFsmInt("npcNumber1");
      if (fsmInt != null)
        fsmInt.Value = GameStatsManager.Instance.huntProgress.ActiveQuest.NumberKillsRequired - GameStatsManager.Instance.huntProgress.CurrentActiveMonsterHuntProgress;
      this.Finish();
    }
  }
}
