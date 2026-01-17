// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGravity2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the gravity vector, or individual axis.")]
[ActionCategory(ActionCategory.Physics2D)]
public class SetGravity2d : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Gravity as Vector2.")]
  public FsmVector2 vector;
  [HutongGames.PlayMaker.Tooltip("Override the x value of the gravity")]
  public FsmFloat x;
  [HutongGames.PlayMaker.Tooltip("Override the y value of the gravity")]
  public FsmFloat y;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
  public bool everyFrame;

  public override void Reset()
  {
    this.vector = (FsmVector2) null;
    FsmFloat fsmFloat1 = new FsmFloat();
    fsmFloat1.UseVariable = true;
    this.x = fsmFloat1;
    FsmFloat fsmFloat2 = new FsmFloat();
    fsmFloat2.UseVariable = true;
    this.y = fsmFloat2;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetGravity();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetGravity();

  private void DoSetGravity()
  {
    Vector2 vector2 = this.vector.Value;
    if (!this.x.IsNone)
      vector2.x = this.x.Value;
    if (!this.y.IsNone)
      vector2.y = this.y.Value;
    Physics2D.gravity = vector2;
  }
}
