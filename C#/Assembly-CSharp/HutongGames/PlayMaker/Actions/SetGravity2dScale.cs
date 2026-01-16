// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGravity2dScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics2D)]
[HutongGames.PlayMaker.Tooltip("Sets The degree to which this object is affected by gravity.  NOTE: Game object must have a rigidbody 2D.")]
public class SetGravity2dScale : ComponentAction<Rigidbody2D>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody 2d attached")]
  [CheckForComponent(typeof (Rigidbody2D))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The gravity scale effect")]
  [RequiredField]
  public FsmFloat gravityScale;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.gravityScale = (FsmFloat) 1f;
  }

  public override void OnEnter()
  {
    this.DoSetGravityScale();
    this.Finish();
  }

  private void DoSetGravityScale()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    this.rigidbody2d.gravityScale = this.gravityScale.Value;
  }
}
