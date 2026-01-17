// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetVelocity2d
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
[ActionCategory(ActionCategory.Physics2D)]
public class GetVelocity2d : ComponentAction<Rigidbody2D>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
  [CheckForComponent(typeof (Rigidbody2D))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The velocity")]
  [UIHint(UIHint.Variable)]
  public FsmVector2 vector;
  [HutongGames.PlayMaker.Tooltip("The x value of the velocity")]
  [UIHint(UIHint.Variable)]
  public FsmFloat x;
  [HutongGames.PlayMaker.Tooltip("The y value of the velocity")]
  [UIHint(UIHint.Variable)]
  public FsmFloat y;
  [HutongGames.PlayMaker.Tooltip("The space reference to express the velocity")]
  public Space space;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.vector = (FsmVector2) null;
    this.x = (FsmFloat) null;
    this.y = (FsmFloat) null;
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
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    Vector2 direction = this.rigidbody2d.velocity;
    if (this.space == Space.Self)
      direction = (Vector2) this.rigidbody2d.transform.InverseTransformDirection((Vector3) direction);
    this.vector.Value = direction;
    this.x.Value = direction.x;
    this.y.Value = direction.y;
  }
}
