using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Starts an NPC conversation (makes the NPC uninteractable).")]
  [ActionCategory(".NPCs")]
  public class BeginConversation : FsmStateAction
  {
    public const float LetterBoxAmount = 0.35f;
    public const float LetterBoxLerpTime = 0.25f;
    public static Vector2 NpcScreenBuffer = new Vector2(0.3f, 0.3f);
    [HutongGames.PlayMaker.Tooltip("Normal: Full conversation, press 'action' to advance.\nPassive: Just a speec bubble over the NPC's head.")]
    public BeginConversation.ConversationType conversationType;
    [HutongGames.PlayMaker.Tooltip("Whether or not to take control away from the player during the conversation.\nDefault will lock normal conversations but not passive conversations.")]
    public BeginConversation.LockedConversation locked;
    [HutongGames.PlayMaker.Tooltip("Whether or not to take control away from the player during the conversation.\nDefault will lock normal conversations but not passive conversations.")]
    public FsmFloat overrideNpcScreenHeight = (FsmFloat) -1f;
    public bool UsesCustomScreenBuffer;
    public Vector2 CustomScreenBuffer;

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      if ((Object) component.TalkingPlayer == (Object) null)
        component.TalkingPlayer = GameManager.Instance.PrimaryPlayer;
      for (int index = 0; index < StaticReferenceManager.AllNpcs.Count; ++index)
      {
        TalkDoerLite allNpc = StaticReferenceManager.AllNpcs[index];
        if ((bool) (Object) allNpc && (Object) allNpc != (Object) component)
          allNpc.CloseTextBox(true);
      }
      GameUIRoot.Instance.InitializeConversationPortrait(component.TalkingPlayer);
      GameUIRoot.Instance.levelNameUI.BanishLevelNameText();
      if (this.conversationType == BeginConversation.ConversationType.Normal)
        component.State = TalkDoerLite.TalkingState.Conversation;
      else if (this.conversationType == BeginConversation.ConversationType.Passive)
        component.State = TalkDoerLite.TalkingState.Passive;
      bool flag = this.locked == BeginConversation.LockedConversation.Locked;
      if (this.locked == BeginConversation.LockedConversation.Default)
        flag = this.conversationType == BeginConversation.ConversationType.Normal;
      if (flag && !component.HasPlayerLocked)
      {
        component.HasPlayerLocked = true;
        component.TalkingPlayer.SetInputOverride("conversation");
        Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
        Pixelator.Instance.DoFinalNonFadedLayer = true;
        GameUIRoot.Instance.ToggleLowerPanels(false, source: "conversation");
        GameUIRoot.Instance.HideCoreUI("conversation");
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
          GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().ForceHideMetaCurrencyPanel();
        Minimap.Instance.TemporarilyPreventMinimap = true;
        Vector2 point = component.speakPoint.transform.position.XY();
        Vector2 vector2_1 = !this.UsesCustomScreenBuffer ? BeginConversation.NpcScreenBuffer : this.CustomScreenBuffer;
        CameraController cameraController = GameManager.Instance.MainCameraController;
        Vector2 world1 = CameraController.CameraToWorld(vector2_1.x, vector2_1.y);
        Vector2 world2 = CameraController.CameraToWorld(1f - vector2_1.x, 1f - vector2_1.y);
        Vector2 vector2_2 = world2 - world1;
        if ((double) this.overrideNpcScreenHeight.Value >= 0.0)
        {
          float y = CameraController.CameraToWorld(0.5f, this.overrideNpcScreenHeight.Value).y;
          world1.y = y;
          world2.y = y;
          vector2_2.y = 0.0f;
        }
        cameraController.SetManualControl(true);
        if (new Rect(world1.x, world1.y, vector2_2.x, vector2_2.y).Contains(point))
        {
          cameraController.OverridePosition = cameraController.transform.position;
        }
        else
        {
          Vector2 vector2_3 = BraveMathCollege.ClosestPointOnRectangle(point, world1, world2 - world1);
          cameraController.OverridePosition = cameraController.transform.position + (Vector3) (point - vector2_3);
        }
      }
      this.Finish();
    }

    public enum ConversationType
    {
      Normal,
      Passive,
    }

    public enum LockedConversation
    {
      Default,
      Locked,
      Unlocked,
    }
  }
}
