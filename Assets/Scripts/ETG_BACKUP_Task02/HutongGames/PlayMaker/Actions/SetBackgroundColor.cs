// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetBackgroundColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Background Color used by the Camera.")]
[ActionCategory(ActionCategory.Camera)]
public class SetBackgroundColor : ComponentAction<Camera>
{
  [CheckForComponent(typeof (Camera))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  public FsmColor backgroundColor;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.backgroundColor = (FsmColor) Color.black;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetBackgroundColor();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetBackgroundColor();

  private void DoSetBackgroundColor()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.camera.backgroundColor = this.backgroundColor.Value;
  }
}
