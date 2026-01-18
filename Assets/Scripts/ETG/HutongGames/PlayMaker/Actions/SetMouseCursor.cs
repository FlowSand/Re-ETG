#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUI)]
  [Tooltip("Controls the appearance of Mouse Cursor.")]
  public class SetMouseCursor : FsmStateAction
  {
    public FsmTexture cursorTexture;
    public FsmBool hideCursor;
    public FsmBool lockCursor;

    public override void Reset()
    {
      this.cursorTexture = (FsmTexture) null;
      this.hideCursor = (FsmBool) false;
      this.lockCursor = (FsmBool) false;
    }

    public override void OnEnter()
    {
      PlayMakerGUI.LockCursor = this.lockCursor.Value;
      PlayMakerGUI.HideCursor = this.hideCursor.Value;
      PlayMakerGUI.MouseCursor = this.cursorTexture.Value;
      this.Finish();
    }
  }
}
