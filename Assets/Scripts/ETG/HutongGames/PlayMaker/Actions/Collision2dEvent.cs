// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Collision2dEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Detect collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, COLLISION ENTER 2D, COLLISION STAY 2D, and COLLISION EXIT 2D are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
  [ActionCategory(ActionCategory.Physics2D)]
  public class Collision2dEvent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The type of collision to detect.")]
    public Collision2DType collision;
    [HutongGames.PlayMaker.Tooltip("Filter by Tag.")]
    [UIHint(UIHint.Tag)]
    public FsmString collideTag;
    [HutongGames.PlayMaker.Tooltip("Event to send if a collision is detected.")]
    public FsmEvent sendEvent;
    [HutongGames.PlayMaker.Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeCollider;
    [HutongGames.PlayMaker.Tooltip("Store the force of the collision. NOTE: Use Get Collision 2D Info to get more info about the collision.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeForce;

    public override void Reset()
    {
      this.collision = Collision2DType.OnCollisionEnter2D;
      this.collideTag = (FsmString) "Untagged";
      this.sendEvent = (FsmEvent) null;
      this.storeCollider = (FsmGameObject) null;
      this.storeForce = (FsmFloat) null;
    }

    public override void OnPreprocess()
    {
      switch (this.collision)
      {
        case Collision2DType.OnCollisionEnter2D:
          this.Fsm.HandleCollisionEnter2D = true;
          break;
        case Collision2DType.OnCollisionStay2D:
          this.Fsm.HandleCollisionStay2D = true;
          break;
        case Collision2DType.OnCollisionExit2D:
          this.Fsm.HandleCollisionExit2D = true;
          break;
        case Collision2DType.OnParticleCollision:
          this.Fsm.HandleParticleCollision = true;
          break;
      }
    }

    private void StoreCollisionInfo(Collision2D collisionInfo)
    {
      this.storeCollider.Value = collisionInfo.gameObject;
      this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
    }

    public override void DoCollisionEnter2D(Collision2D collisionInfo)
    {
      if (this.collision != Collision2DType.OnCollisionEnter2D || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
        return;
      this.StoreCollisionInfo(collisionInfo);
      this.Fsm.Event(this.sendEvent);
    }

    public override void DoCollisionStay2D(Collision2D collisionInfo)
    {
      if (this.collision != Collision2DType.OnCollisionStay2D || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
        return;
      this.StoreCollisionInfo(collisionInfo);
      this.Fsm.Event(this.sendEvent);
    }

    public override void DoCollisionExit2D(Collision2D collisionInfo)
    {
      if (this.collision != Collision2DType.OnCollisionExit2D || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
        return;
      this.StoreCollisionInfo(collisionInfo);
      this.Fsm.Event(this.sendEvent);
    }

    public override void DoParticleCollision(GameObject other)
    {
      if (this.collision != Collision2DType.OnParticleCollision || !(other.tag == this.collideTag.Value))
        return;
      if (this.storeCollider != null)
        this.storeCollider.Value = other;
      this.storeForce.Value = 0.0f;
      this.Fsm.Event(this.sendEvent);
    }

    public override string ErrorCheck() => ActionHelpers.CheckOwnerPhysics2dSetup(this.Owner);
  }
}
