// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetCollisionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Gets info on the last collision event and store in variables. See Unity Physics docs.")]
[ActionCategory(ActionCategory.Physics)]
public class GetCollisionInfo : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [Tooltip("Get the GameObject hit.")]
  public FsmGameObject gameObjectHit;
  [UIHint(UIHint.Variable)]
  [Tooltip("Get the relative velocity of the collision.")]
  public FsmVector3 relativeVelocity;
  [UIHint(UIHint.Variable)]
  [Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
  public FsmFloat relativeSpeed;
  [UIHint(UIHint.Variable)]
  [Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
  public FsmVector3 contactPoint;
  [UIHint(UIHint.Variable)]
  [Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
  public FsmVector3 contactNormal;
  [UIHint(UIHint.Variable)]
  [Tooltip("Get the name of the physics material of the colliding GameObject. Useful for triggering different effects. Audio, particles...")]
  public FsmString physicsMaterialName;

  public override void Reset()
  {
    this.gameObjectHit = (FsmGameObject) null;
    this.relativeVelocity = (FsmVector3) null;
    this.relativeSpeed = (FsmFloat) null;
    this.contactPoint = (FsmVector3) null;
    this.contactNormal = (FsmVector3) null;
    this.physicsMaterialName = (FsmString) null;
  }

  private void StoreCollisionInfo()
  {
    if (this.Fsm.CollisionInfo == null)
      return;
    this.gameObjectHit.Value = this.Fsm.CollisionInfo.gameObject;
    this.relativeSpeed.Value = this.Fsm.CollisionInfo.relativeVelocity.magnitude;
    this.relativeVelocity.Value = this.Fsm.CollisionInfo.relativeVelocity;
    this.physicsMaterialName.Value = this.Fsm.CollisionInfo.collider.material.name;
    if (this.Fsm.CollisionInfo.contacts == null || this.Fsm.CollisionInfo.contacts.Length <= 0)
      return;
    this.contactPoint.Value = this.Fsm.CollisionInfo.contacts[0].point;
    this.contactNormal.Value = this.Fsm.CollisionInfo.contacts[0].normal;
  }

  public override void OnEnter()
  {
    this.StoreCollisionInfo();
    this.Finish();
  }
}
