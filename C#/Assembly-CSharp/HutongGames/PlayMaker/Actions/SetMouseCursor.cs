// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetMouseCursor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

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
