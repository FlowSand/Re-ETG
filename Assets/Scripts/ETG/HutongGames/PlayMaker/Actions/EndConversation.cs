using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Ends an NPC conversation (makes the NPC interactable).")]
  [ActionCategory(".NPCs")]
  public class EndConversation : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("If true, force closes all text boxes, even zombie text boxes.")]
    public FsmBool killZombieTextBoxes;
    public FsmBool doNotLerpCamera;
    public FsmBool suppressReinteractDelay;
    public FsmBool suppressFurtherInteraction;

    public override void Reset() => this.killZombieTextBoxes = (FsmBool) false;

    public static void ForceEndConversation(TalkDoerLite talkDoer)
    {
      if ((Object) talkDoer.TalkingPlayer != (Object) null && talkDoer.State == TalkDoerLite.TalkingState.Conversation)
        talkDoer.CompletedTalkingPlayer = (double) Vector2.Distance(talkDoer.TalkingPlayer.sprite.WorldCenter, talkDoer.sprite.WorldCenter) > (double) talkDoer.conversationBreakRadius ? (PlayerController) null : talkDoer.TalkingPlayer;
      if (talkDoer.HasPlayerLocked)
      {
        talkDoer.TalkingPlayer.ClearInputOverride("conversation");
        talkDoer.HasPlayerLocked = false;
        Pixelator.Instance.LerpToLetterbox(0.5f, 0.25f);
        Pixelator.Instance.DoFinalNonFadedLayer = false;
        GameUIRoot.Instance.ToggleLowerPanels(true, source: "conversation");
        GameUIRoot.Instance.ShowCoreUI("conversation");
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
          GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().ForceRevealMetaCurrencyPanel();
        Minimap.Instance.TemporarilyPreventMinimap = false;
        GameManager.Instance.MainCameraController.SetManualControl(false);
      }
      if ((bool) (Object) talkDoer.TalkingPlayer)
        TextBoxManager.ClearTextBox(talkDoer.TalkingPlayer.transform);
      talkDoer.IsTalking = false;
      talkDoer.TalkingPlayer = (PlayerController) null;
      talkDoer.CloseTextBox(true);
    }

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      if (!(bool) (Object) component)
        return;
      if ((Object) component.TalkingPlayer != (Object) null && component.State == TalkDoerLite.TalkingState.Conversation)
        component.CompletedTalkingPlayer = (double) Vector2.Distance(component.TalkingPlayer.sprite.WorldCenter, component.sprite.WorldCenter) > (double) component.conversationBreakRadius ? (PlayerController) null : component.TalkingPlayer;
      if (component.HasPlayerLocked)
      {
        component.TalkingPlayer.ClearInputOverride("conversation");
        component.HasPlayerLocked = false;
        Pixelator.Instance.LerpToLetterbox(0.5f, 0.25f);
        Pixelator.Instance.DoFinalNonFadedLayer = false;
        GameUIRoot.Instance.ToggleLowerPanels(true, source: "conversation");
        GameUIRoot.Instance.ShowCoreUI("conversation");
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
          GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().ForceRevealMetaCurrencyPanel();
        Minimap.Instance.TemporarilyPreventMinimap = false;
        if (!this.doNotLerpCamera.Value)
          GameManager.Instance.MainCameraController.SetManualControl(false);
      }
      if (this.suppressReinteractDelay.Value)
        component.SuppressReinteractDelay = true;
      if ((bool) (Object) component.TalkingPlayer)
        TextBoxManager.ClearTextBox(component.TalkingPlayer.transform);
      this.ClearAlternativeTalkerFromPrevious();
      component.IsTalking = false;
      component.TalkingPlayer = (PlayerController) null;
      component.CloseTextBox(this.killZombieTextBoxes.Value);
      if (this.suppressReinteractDelay.Value)
        component.SuppressReinteractDelay = false;
      if (this.suppressFurtherInteraction.Value)
        component.ForceNonInteractable = true;
      this.Finish();
    }

    private void ClearAlternativeTalkerFromPrevious()
    {
      FsmState previousActiveState = this.Fsm.PreviousActiveState;
      if (previousActiveState == null)
        return;
      for (int index = 0; index < previousActiveState.Actions.Length; ++index)
      {
        if (previousActiveState.Actions[index] is DialogueBox)
        {
          DialogueBox action = previousActiveState.Actions[index] as DialogueBox;
          if ((Object) action.AlternativeTalker != (Object) null)
          {
            action.AlternativeTalker.SuppressClear = false;
            TextBoxManager.ClearTextBox(action.AlternativeTalker.speakPoint);
          }
        }
      }
    }
  }
}
