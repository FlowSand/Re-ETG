// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetVelocity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Physics)]
[HutongGames.PlayMaker.Tooltip("Gets the Velocity of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body.")]
public class GetVelocity : ComponentAction<Rigidbody>
{
  [CheckForComponent(typeof (Rigidbody))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  public FsmVector3 vector;
  [UIHint(UIHint.Variable)]
  public FsmFloat x;
  [UIHint(UIHint.Variable)]
  public FsmFloat y;
  [UIHint(UIHint.Variable)]
  public FsmFloat z;
  public Space space;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.vector = (FsmVector3) null;
    this.x = (FsmFloat) null;
    this.y = (FsmFloat) null;
    this.z = (FsmFloat) null;
    this.space = Space.World;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetVelocity();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetVelocity();

  private void DoGetVelocity()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if (!this.UpdateCache(ownerDefaultTarget))
      return;
    Vector3 direction = this.rigidbody.velocity;
    if (this.space == Space.Self)
      direction = ownerDefaultTarget.transform.InverseTransformDirection(direction);
    this.vector.Value = direction;
    this.x.Value = direction.x;
    this.y.Value = direction.y;
    this.z.Value = direction.z;
  }
}
