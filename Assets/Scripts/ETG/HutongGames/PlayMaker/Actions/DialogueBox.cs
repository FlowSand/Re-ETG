using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [HutongGames.PlayMaker.Tooltip("Opens a dialogue box and speaks one or more lines of dialogue. Also supports one set of player responses.\nOnly the first valid Dialogue Box action will be run for a given state.")]
  public class DialogueBox : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Only show this dialogue box if this condition is met")]
    public DialogueBox.Condition condition;
    [HutongGames.PlayMaker.Tooltip("Handles the dialogue sequence.")]
    [ActionSection("Text")]
    public DialogueBox.DialogueSequence sequence;
    [HutongGames.PlayMaker.Tooltip("The number of persistent strings to show for each key before progressing to the next one.")]
    public FsmInt persistentStringsToShow = (FsmInt) 1;
    [HutongGames.PlayMaker.Tooltip("Dialogue strings for the NPC to say.")]
    public FsmString[] dialogue;
    [CompoundArray("Responses", "Text", "Event")]
    public FsmString[] responses;
    public FsmEvent[] events;
    [HutongGames.PlayMaker.Tooltip("If true, player distance will not cause the playerWalkedAway event to fire.")]
    [ActionSection("Advanced")]
    public FsmBool skipWalkAwayEvent;
    [HutongGames.PlayMaker.Tooltip("If set, after this amount of time (seconds) the dialogue box will force close.")]
    public FsmFloat forceCloseTime;
    [HutongGames.PlayMaker.Tooltip("If set, after the dialogue box closes it will remain up for this amount of time (seconds). Set to -1 to leave it up until something else overrides it.")]
    public FsmFloat zombieTime;
    [HutongGames.PlayMaker.Tooltip("If true, don't use the default talk and idle animations.")]
    public FsmBool SuppressDefaultAnims;
    [HutongGames.PlayMaker.Tooltip("If specified, use this animation instead of the default talk animation.")]
    public FsmString OverrideTalkAnim;
    [HutongGames.PlayMaker.Tooltip("If marked, play the textbox over the player. Only for Pasts!")]
    public FsmBool PlayBoxOnInteractingPlayer;
    [HutongGames.PlayMaker.Tooltip("Thot box")]
    public FsmBool IsThoughtBubble;
    [HutongGames.PlayMaker.Tooltip("If used, play the textbox over this talk doer instead.")]
    public TalkDoerLite AlternativeTalker;
    private TalkDoerLite m_talkDoer;
    private DialogueBox.DialogueState m_dialogueState;
    private int m_numDialogues;
    private int m_textIndex;
    private int m_persistentIndex;
    private float m_forceCloseTimer;
    private string[] m_rawResponses;
    private int m_sequentialStringLastIndex = -1;
    private string m_currentDialogueText;

    public override void Reset()
    {
      this.condition = DialogueBox.Condition.All;
      this.sequence = DialogueBox.DialogueSequence.Default;
      this.persistentStringsToShow = (FsmInt) 1;
      this.dialogue = new FsmString[1]
      {
        new FsmString(string.Empty)
      };
      this.responses = (FsmString[]) null;
      this.events = (FsmEvent[]) null;
      this.skipWalkAwayEvent = (FsmBool) false;
      this.forceCloseTime = (FsmFloat) 0.0f;
      this.zombieTime = (FsmFloat) 0.0f;
      this.SuppressDefaultAnims = (FsmBool) false;
      this.OverrideTalkAnim = (FsmString) string.Empty;
      this.PlayBoxOnInteractingPlayer = (FsmBool) false;
      this.AlternativeTalker = (TalkDoerLite) null;
    }

    public override string ErrorCheck()
    {
      string empty = string.Empty;
      AIAnimator component = this.Owner.GetComponent<AIAnimator>();
      if (!this.SuppressDefaultAnims.Value)
      {
        if (!(bool) (Object) component)
          empty += "Owner must have an AIAnimator to manage animations to use default animations.";
        if ((bool) (Object) component && !component.HasDefaultAnimation)
          empty += "AIAnimator must have a default (base or idle) animation to use default animations.";
        if ((bool) (Object) component && !component.HasDirectionalAnimation("talk"))
          empty += "AIAnimator must have a talk animation to use default animations.";
      }
      if (this.sequence == DialogueBox.DialogueSequence.Mutliline && this.dialogue.Length != 1)
        empty += "Multiline only supports a single dialogue string.\n";
      if (this.sequence == DialogueBox.DialogueSequence.Sequential && this.dialogue.Length != 1)
        empty += "Sequential only supports a single dialogue string.\n";
      if (this.sequence == DialogueBox.DialogueSequence.SeqThenRepeatLast && this.dialogue.Length != 1)
        empty += "SeqThenRepeatLast only supports a single dialogue string.\n";
      if (this.sequence == DialogueBox.DialogueSequence.SeqThenRemoveState && this.dialogue.Length != 1)
        empty += "SeqThenRemoveState only supports a single dialogue string.\n";
      if (this.sequence == DialogueBox.DialogueSequence.PersistentSequential && this.dialogue.Length < 2)
        empty += "PersistentSequential needs at least one sequential dialogue string and one stopper string.\n";
      if (this.dialogue != null && this.dialogue.Length == 0)
        empty += "Dialogue strings must contain at least one line of dialogue.\n";
      if ((double) this.forceCloseTime.Value > 0.0 && this.responses != null && this.responses.Length != 0)
        empty += "Force Close Timer will be ignored if there are dialogue responses.\n";
      return empty;
    }

    public override void OnEnter()
    {
      this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
      if (this.ShouldSkip())
      {
        this.Finish();
      }
      else
      {
        this.m_dialogueState = DialogueBox.DialogueState.ShowNextDialogue;
        if (this.sequence != DialogueBox.DialogueSequence.PersistentSequential)
          this.m_textIndex = 0;
        this.m_forceCloseTimer = 0.0f;
        if (this.skipWalkAwayEvent.Value)
          this.m_talkDoer.AllowWalkAways = false;
        this.m_numDialogues = this.sequence != DialogueBox.DialogueSequence.Default ? (this.sequence != DialogueBox.DialogueSequence.Mutliline ? 1 : StringTableManager.GetNumStrings(this.dialogue[0].Value)) : this.dialogue.Length;
        this.m_rawResponses = new string[this.responses.Length];
        for (int index = 0; index < this.responses.Length; ++index)
        {
          this.m_rawResponses[index] = StringTableManager.GetString(this.responses[index].Value);
          this.m_rawResponses[index] = this.NPCReplacementPostprocessString(this.m_rawResponses[index]);
        }
      }
    }

    public override void OnUpdate()
    {
      if (this.m_dialogueState == DialogueBox.DialogueState.ShowNextDialogue)
      {
        this.NextDialogue();
        this.m_dialogueState = DialogueBox.DialogueState.ShowingDialogue;
        if (this.SuppressDefaultAnims.Value)
          return;
        if ((Object) this.AlternativeTalker != (Object) null)
          this.AlternativeTalker.aiAnimator.PlayUntilFinished(this.TalkAnimName);
        else
          this.m_talkDoer.aiAnimator.PlayUntilFinished(this.TalkAnimName);
      }
      else if (this.m_dialogueState == DialogueBox.DialogueState.ShowingDialogue)
      {
        bool flag1 = false;
        if (this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation)
        {
          bool suppressThisClick;
          flag1 = BraveInput.GetInstanceForPlayer(this.m_talkDoer.TalkingPlayer.PlayerIDX).WasAdvanceDialoguePressed(out suppressThisClick);
          if (suppressThisClick)
            this.m_talkDoer.TalkingPlayer.SuppressThisClick = true;
        }
        bool flag2 = false;
        if ((double) this.m_forceCloseTimer > 0.0)
        {
          this.m_forceCloseTimer -= BraveTime.DeltaTime;
          flag2 = (double) this.m_forceCloseTimer <= 0.0;
        }
        if (flag1 || flag2)
        {
          if (TextBoxManager.TextBoxCanBeAdvanced(this.m_talkDoer.speakPoint) && !flag2)
            TextBoxManager.AdvanceTextBox(this.m_talkDoer.speakPoint);
          else if (this.m_textIndex < this.m_numDialogues && this.sequence != DialogueBox.DialogueSequence.PersistentSequential)
          {
            if ((Object) this.m_talkDoer.echo1 != (Object) null)
              this.m_talkDoer.echo1.IsDoingForcedSpeech = false;
            if ((Object) this.m_talkDoer.echo2 != (Object) null)
              this.m_talkDoer.echo2.IsDoingForcedSpeech = false;
            this.m_dialogueState = DialogueBox.DialogueState.ShowNextDialogue;
          }
          else if (this.responses.Length > 0)
          {
            this.m_dialogueState = DialogueBox.DialogueState.ShowingResponses;
          }
          else
          {
            if ((double) this.forceCloseTime.Value != 0.0 && (double) this.zombieTime.Value != 0.0)
              this.m_talkDoer.SetZombieBoxTimer(Mathf.Max(Mathf.Max(this.forceCloseTime.Value + this.zombieTime.Value, (float) (0.5 + (double) TextBoxManager.GetEstimatedReadingTime(this.m_currentDialogueText) * (double) TextBoxManager.ZombieBoxMultiplier)) - this.forceCloseTime.Value, 0.1f), this.TalkAnimName);
            else if (!this.SuppressDefaultAnims.Value)
            {
              if ((Object) this.AlternativeTalker != (Object) null)
                this.AlternativeTalker.aiAnimator.EndAnimationIf(this.TalkAnimName);
              else
                this.m_talkDoer.aiAnimator.EndAnimationIf(this.TalkAnimName);
            }
            this.Finish();
          }
        }
        else
        {
          if (this.responses.Length <= 0 || this.m_textIndex != this.m_numDialogues || TextBoxManager.TextBoxCanBeAdvanced(this.m_talkDoer.speakPoint))
            return;
          this.m_dialogueState = DialogueBox.DialogueState.ShowingResponses;
        }
      }
      else if (this.m_dialogueState == DialogueBox.DialogueState.ShowingResponses)
      {
        this.ShowResponses();
        this.m_dialogueState = DialogueBox.DialogueState.WaitingForResponse;
      }
      else
      {
        int responseIndex;
        if (this.m_dialogueState != DialogueBox.DialogueState.WaitingForResponse || !GameUIRoot.Instance.GetPlayerConversationResponse(out responseIndex))
          return;
        this.m_talkDoer.TalkingPlayer.ClearInputOverride("dialogueResponse");
        this.m_talkDoer.CloseTextBox(true);
        this.Finish();
        if (!this.SuppressDefaultAnims.Value)
        {
          if ((Object) this.AlternativeTalker != (Object) null)
            this.AlternativeTalker.aiAnimator.EndAnimationIf(this.TalkAnimName);
          else
            this.m_talkDoer.aiAnimator.EndAnimationIf(this.TalkAnimName);
        }
        this.Fsm.Event(this.events[responseIndex]);
      }
    }

    public override void OnExit()
    {
      if (!(bool) (Object) this.m_talkDoer)
        return;
      this.m_talkDoer.CloseTextBox(false);
      if (!this.skipWalkAwayEvent.Value)
        return;
      this.m_talkDoer.AllowWalkAways = true;
    }

    private bool ShouldSkip()
    {
      if (this.condition == DialogueBox.Condition.FirstEncounterThisInstance)
      {
        if (this.m_talkDoer.NumTimesSpokenTo > 1)
          return true;
      }
      else if (this.condition == DialogueBox.Condition.FirstEverEncounter)
      {
        EncounterTrackable component = this.Owner.GetComponent<EncounterTrackable>();
        if ((Object) component == (Object) null || GameStatsManager.Instance.QueryEncounterable(component) > 1)
          return true;
      }
      else if (this.condition == DialogueBox.Condition.KeyboardAndMouse)
      {
        if (!BraveInput.GetInstanceForPlayer(this.m_talkDoer.TalkingPlayer.PlayerIDX).IsKeyboardAndMouse())
          return true;
      }
      else if (this.condition == DialogueBox.Condition.Controller && BraveInput.GetInstanceForPlayer(this.m_talkDoer.TalkingPlayer.PlayerIDX).IsKeyboardAndMouse())
        return true;
      for (int index = 0; index < this.State.Actions.Length && this.State.Actions[index] != this; ++index)
      {
        if (this.State.Actions[index] is DialogueBox && this.State.Actions[index].Active)
          return true;
      }
      return false;
    }

    private string NPCReplacementPostprocessString(string input)
    {
      FsmString fsmString = this.Fsm.Variables.GetFsmString("npcReplacementString");
      if (fsmString != null && !string.IsNullOrEmpty(fsmString.Value))
        input = input.Replace("%NPCREPLACEMENT", fsmString.Value);
      string oldValue = "%NPCNUMBER1";
      int num = 1;
      for (; input.Contains(oldValue); oldValue = "%NPCNUMBER" + num.ToString())
      {
        FsmInt fsmInt = this.Fsm.Variables.GetFsmInt("npcNumber" + num.ToString());
        if (fsmInt != null)
          input = input.Replace(oldValue, fsmInt.Value.ToString());
        ++num;
      }
      return input;
    }

    private void NextDialogue()
    {
      if (this.m_textIndex <= 0)
        ;
      bool flag1 = this.m_textIndex == this.m_numDialogues - 1;
      string input = "ERROR ERROR";
      if (this.m_textIndex < this.dialogue.Length && this.m_textIndex >= 0 && this.dialogue[this.m_textIndex].UsesVariable && !this.dialogue[this.m_textIndex].Value.StartsWith("#"))
        input = this.dialogue[this.m_textIndex].Value;
      else if (this.sequence == DialogueBox.DialogueSequence.Default)
        input = StringTableManager.GetString(this.dialogue[this.m_textIndex].Value);
      else if (this.sequence == DialogueBox.DialogueSequence.Mutliline)
        input = StringTableManager.GetExactString(this.dialogue[0].Value, this.m_textIndex);
      else if (this.sequence == DialogueBox.DialogueSequence.SeqThenRemoveState)
      {
        bool isLast;
        input = StringTableManager.GetStringSequential(this.dialogue[0].Value, ref this.m_sequentialStringLastIndex, out isLast);
        if (isLast)
          BravePlayMakerUtility.DisconnectState(this.State);
      }
      else if (this.sequence == DialogueBox.DialogueSequence.Sequential || this.sequence == DialogueBox.DialogueSequence.SeqThenRepeatLast)
        input = StringTableManager.GetStringSequential(this.dialogue[0].Value, ref this.m_sequentialStringLastIndex, this.sequence == DialogueBox.DialogueSequence.SeqThenRepeatLast);
      else if (this.sequence == DialogueBox.DialogueSequence.PersistentSequential)
      {
        if (this.m_textIndex < this.dialogue.Length - 1)
        {
          input = StringTableManager.GetStringPersistentSequential(this.dialogue[this.m_textIndex].Value);
        }
        else
        {
          input = StringTableManager.GetString(this.dialogue[this.m_textIndex].Value);
          flag1 = true;
        }
      }
      if (input.Contains("$"))
      {
        string[] strArray = input.Split('$');
        input = strArray[0];
        if (strArray.Length > 1)
        {
          for (int index = 1; index < strArray.Length && index - 1 < this.m_rawResponses.Length; ++index)
            this.m_rawResponses[index - 1] = strArray[index];
        }
      }
      else if (input.Contains("&"))
      {
        string[] strArray = input.Split('&');
        input = strArray[0];
        if ((Object) this.m_talkDoer.echo1 != (Object) null)
          this.m_talkDoer.echo1.ForceTimedSpeech(strArray[1], 1f, 4f, TextBoxManager.BoxSlideOrientation.FORCE_RIGHT);
        if ((Object) this.m_talkDoer.echo2 != (Object) null && strArray.Length > 2)
          this.m_talkDoer.echo2.ForceTimedSpeech(strArray[2], 2f, 4f, TextBoxManager.BoxSlideOrientation.FORCE_LEFT);
      }
      string str1 = this.NPCReplacementPostprocessString(input);
      this.m_currentDialogueText = str1;
      this.ClearAlternativeTalkerFromPrevious();
      if ((Object) this.AlternativeTalker != (Object) null)
      {
        this.AlternativeTalker.SuppressClear = true;
        TextBoxManager.ClearTextBox(this.m_talkDoer.speakPoint);
        TextBoxManager.ClearTextBox(this.m_talkDoer.TalkingPlayer.transform);
        TalkDoerLite talkDoer = this.m_talkDoer;
        Vector3 vector3 = this.AlternativeTalker.speakPoint.position + new Vector3(0.0f, 0.0f, -5f);
        Transform speakPoint = this.AlternativeTalker.speakPoint;
        float num1 = -1f;
        string str2 = str1;
        bool flag2 = false;
        bool flag3 = this.HasNextDialogue() && this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
        Vector3 worldPosition = vector3;
        Transform parent = speakPoint;
        double duration = (double) num1;
        string text = str2;
        int num2 = flag2 ? 1 : 0;
        int num3 = flag3 ? 1 : 0;
        int num4 = this.IsThoughtBubble.Value ? 1 : 0;
        string characterSpeechTag = this.AlternativeTalker.audioCharacterSpeechTag;
        talkDoer.ShowText(worldPosition, parent, (float) duration, text, num2 != 0, showContinueText: num3 != 0, isThoughtBox: num4 != 0, overrideSpeechAudioTag: characterSpeechTag);
      }
      else if (this.PlayBoxOnInteractingPlayer.Value)
      {
        TextBoxManager.ClearTextBox(this.m_talkDoer.speakPoint);
        TalkDoerLite talkDoer = this.m_talkDoer;
        Vector3 vector3 = this.m_talkDoer.TalkingPlayer.CenterPosition.ToVector3ZUp(this.m_talkDoer.TalkingPlayer.CenterPosition.y) + new Vector3(0.0f, 1f, -5f);
        Transform transform = this.m_talkDoer.TalkingPlayer.transform;
        float num5 = -1f;
        string str3 = str1;
        bool flag4 = false;
        bool flag5 = this.HasNextDialogue() && this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
        Vector3 worldPosition = vector3;
        Transform parent = transform;
        double duration = (double) num5;
        string text = str3;
        int num6 = flag4 ? 1 : 0;
        int num7 = flag5 ? 1 : 0;
        int num8 = this.IsThoughtBubble.Value ? 1 : 0;
        string characterAudioSpeechTag = this.m_talkDoer.TalkingPlayer.characterAudioSpeechTag;
        talkDoer.ShowText(worldPosition, parent, (float) duration, text, num6 != 0, showContinueText: num7 != 0, isThoughtBox: num8 != 0, overrideSpeechAudioTag: characterAudioSpeechTag);
      }
      else
      {
        if ((bool) (Object) this.m_talkDoer.TalkingPlayer)
          TextBoxManager.ClearTextBox(this.m_talkDoer.TalkingPlayer.transform);
        TalkDoerLite talkDoer = this.m_talkDoer;
        Vector3 vector3 = this.m_talkDoer.speakPoint.position + new Vector3(0.0f, 0.0f, -5f);
        Transform speakPoint = this.m_talkDoer.speakPoint;
        float num9 = -1f;
        string str4 = str1;
        bool flag6 = false;
        bool flag7 = this.HasNextDialogue() && this.m_talkDoer.State == TalkDoerLite.TalkingState.Conversation;
        Vector3 worldPosition = vector3;
        Transform parent = speakPoint;
        double duration = (double) num9;
        string text = str4;
        int num10 = flag6 ? 1 : 0;
        int num11 = flag7 ? 1 : 0;
        int num12 = this.IsThoughtBubble.Value ? 1 : 0;
        talkDoer.ShowText(worldPosition, parent, (float) duration, text, num10 != 0, showContinueText: num11 != 0, isThoughtBox: num12 != 0);
      }
      if (flag1 && (double) this.forceCloseTime.Value > 0.0)
        this.m_forceCloseTimer = this.forceCloseTime.Value;
      if (this.sequence == DialogueBox.DialogueSequence.PersistentSequential)
      {
        ++this.m_persistentIndex;
        if (this.m_persistentIndex < this.persistentStringsToShow.Value)
          return;
        this.m_persistentIndex = 0;
        this.m_textIndex = Mathf.Min(this.m_textIndex + 1, this.dialogue.Length - 1);
      }
      else
        ++this.m_textIndex;
    }

    private void ClearAlternativeTalkerFromPrevious()
    {
      FsmState previousActiveState = this.Fsm.PreviousActiveState;
      if (previousActiveState == null || previousActiveState == this.State)
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

    private void ShowResponses()
    {
      if ((Object) this.m_talkDoer.echo1 != (Object) null)
        this.m_talkDoer.echo1.IsDoingForcedSpeech = false;
      if ((Object) this.m_talkDoer.echo2 != (Object) null)
        this.m_talkDoer.echo2.IsDoingForcedSpeech = false;
      if (this.responses.Length <= 0)
        return;
      this.m_talkDoer.TalkingPlayer.SetInputOverride("dialogueResponse");
      GameUIRoot.Instance.DisplayPlayerConversationOptions(this.m_talkDoer.TalkingPlayer, this.m_rawResponses);
    }

    private bool HasNextDialogue()
    {
      if (this.sequence == DialogueBox.DialogueSequence.PersistentSequential)
        return false;
      if (this.m_textIndex < this.m_numDialogues - 1)
        return true;
      for (int index1 = 0; index1 < this.State.Transitions.Length; ++index1)
      {
        if (!string.IsNullOrEmpty(this.State.Transitions[index1].ToState))
        {
          FsmState state = this.Fsm.GetState(this.State.Transitions[index1].ToState);
          for (int index2 = 0; index2 < state.Actions.Length; ++index2)
          {
            if (state.Actions[index2] is DialogueBox)
              return true;
          }
        }
      }
      return false;
    }

    private string TalkAnimName
    {
      get
      {
        return this.SuppressDefaultAnims.Value || string.IsNullOrEmpty(this.OverrideTalkAnim.Value) ? "talk" : this.OverrideTalkAnim.Value;
      }
    }

    private enum DialogueState
    {
      ShowNextDialogue,
      ShowingDialogue,
      ShowingResponses,
      WaitingForResponse,
    }

    public enum DialogueSequence
    {
      Default,
      Sequential,
      SeqThenRepeatLast,
      SeqThenRemoveState,
      Mutliline,
      PersistentSequential,
    }

    public enum Condition
    {
      All = 0,
      FirstEncounterThisInstance = 1,
      FirstEverEncounter = 2,
      KeyboardAndMouse = 100, // 0x00000064
      Controller = 110, // 0x0000006E
    }
  }
}
