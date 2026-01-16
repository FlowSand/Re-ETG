// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetSpeed2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the 2d Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body 2D.")]
[ActionCategory(ActionCategory.Physics2D)]
public class GetSpeed2d : ComponentAction<Rigidbody2D>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
  [CheckForComponent(typeof (Rigidbody2D))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The speed, or in technical terms: velocity magnitude")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.storeResult = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetSpeed();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetSpeed();

  private void DoGetSpeed()
  {
    if (this.storeResult.IsNone || !this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.storeResult.Value = this.rigidbody2d.velocity.magnitude;
  }
}
