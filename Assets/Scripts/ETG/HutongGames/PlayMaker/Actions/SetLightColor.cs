// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetLightColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Color of a Light.")]
[ActionCategory(ActionCategory.Lights)]
public class SetLightColor : ComponentAction<Light>
{
  [CheckForComponent(typeof (Light))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  public FsmColor lightColor;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.lightColor = (FsmColor) Color.white;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetLightColor();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetLightColor();

  private void DoSetLightColor()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.light.color = this.lightColor.Value;
  }
}
