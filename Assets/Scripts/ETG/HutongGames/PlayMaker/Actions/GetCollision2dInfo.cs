// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetCollision2dInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets info on the last collision 2D event and store in variables. See Unity and PlayMaker docs on Unity 2D physics.")]
  [ActionCategory(ActionCategory.Physics2D)]
  public class GetCollision2dInfo : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Get the GameObject hit.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject gameObjectHit;
    [HutongGames.PlayMaker.Tooltip("Get the relative velocity of the collision.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 relativeVelocity;
    [HutongGames.PlayMaker.Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat relativeSpeed;
    [HutongGames.PlayMaker.Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 contactPoint;
    [HutongGames.PlayMaker.Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 contactNormal;
    [HutongGames.PlayMaker.Tooltip("The number of separate shaped regions in the collider.")]
    [UIHint(UIHint.Variable)]
    public FsmInt shapeCount;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the name of the physics 2D material of the colliding GameObject. Useful for triggering different effects. Audio, particles...")]
    public FsmString physics2dMaterialName;

    public override void Reset()
    {
      this.gameObjectHit = (FsmGameObject) null;
      this.relativeVelocity = (FsmVector3) null;
      this.relativeSpeed = (FsmFloat) null;
      this.contactPoint = (FsmVector3) null;
      this.contactNormal = (FsmVector3) null;
      this.shapeCount = (FsmInt) null;
      this.physics2dMaterialName = (FsmString) null;
    }

    private void StoreCollisionInfo()
    {
      if (this.Fsm.Collision2DInfo == null)
        return;
      this.gameObjectHit.Value = this.Fsm.Collision2DInfo.gameObject;
      this.relativeSpeed.Value = this.Fsm.Collision2DInfo.relativeVelocity.magnitude;
      this.relativeVelocity.Value = (Vector3) this.Fsm.Collision2DInfo.relativeVelocity;
      this.physics2dMaterialName.Value = !((Object) this.Fsm.Collision2DInfo.collider.sharedMaterial != (Object) null) ? string.Empty : this.Fsm.Collision2DInfo.collider.sharedMaterial.name;
      this.shapeCount.Value = this.Fsm.Collision2DInfo.collider.shapeCount;
      if (this.Fsm.Collision2DInfo.contacts == null || this.Fsm.Collision2DInfo.contacts.Length <= 0)
        return;
      this.contactPoint.Value = (Vector3) this.Fsm.Collision2DInfo.contacts[0].point;
      this.contactNormal.Value = (Vector3) this.Fsm.Collision2DInfo.contacts[0].normal;
    }

    public override void OnEnter()
    {
      this.StoreCollisionInfo();
      this.Finish();
    }
  }
}
