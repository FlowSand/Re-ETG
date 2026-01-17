// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetKicked
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Kickable corpse.")]
[ActionCategory(".Brave")]
public class GetKicked : FsmStateAction
{
  public FsmOwnerDefault GameObject;
  private int kickCount = 1;
  private bool m_hasInitializedSRB;
  private bool m_isFalling;

  public override void Reset() => base.Reset();

  public override void Awake()
  {
    base.Awake();
    UnityEngine.GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.GameObject);
    if (!(bool) (UnityEngine.Object) ownerDefaultTarget)
      return;
    SpeculativeRigidbody component = ownerDefaultTarget.GetComponent<SpeculativeRigidbody>();
    if (!(bool) (UnityEngine.Object) component || this.m_hasInitializedSRB)
      return;
    this.m_hasInitializedSRB = true;
    component.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.HandlePostRigidbodyMotion);
    component.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
    component.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.HandleTileCollision);
  }

  public override void OnEnter()
  {
    UnityEngine.GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.GameObject);
    TalkDoerLite component1 = this.Owner.GetComponent<TalkDoerLite>();
    ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
    AIAnimator component2 = ownerDefaultTarget.GetComponent<AIAnimator>();
    PlayerController interactor = !(bool) (UnityEngine.Object) component1.TalkingPlayer ? GameManager.Instance.PrimaryPlayer : component1.TalkingPlayer;
    if ((bool) (UnityEngine.Object) component2)
    {
      component2.PlayUntilCancelled("kick" + this.kickCount.ToString(), true);
      this.kickCount = this.kickCount % 8 + 1;
      if ((bool) (UnityEngine.Object) component2.specRigidbody && (bool) (UnityEngine.Object) interactor)
      {
        SpeculativeRigidbody specRigidbody = component2.specRigidbody;
        if (!this.m_hasInitializedSRB)
        {
          this.m_hasInitializedSRB = true;
          specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.HandlePostRigidbodyMotion);
          specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
          specRigidbody.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.HandleTileCollision);
        }
        specRigidbody.Velocity += (specRigidbody.UnitCenter - interactor.CenterPosition).normalized * 3f;
        SpawnManager.SpawnVFX((UnityEngine.GameObject) BraveResources.Load("Global VFX/VFX_DodgeRollHit"), (Vector3) specRigidbody.UnitCenter, Quaternion.identity, true);
      }
    }
    if ((bool) (UnityEngine.Object) interactor)
      this.SetAnimationState(interactor, component1);
    this.Finish();
  }

  private void HandleTileCollision(CollisionData tileCollision)
  {
    Vector2 velocity = tileCollision.MyRigidbody.Velocity;
    float angle1 = (-velocity).ToAngle();
    float angle2 = tileCollision.Normal.ToAngle();
    PhysicsEngine.PostSliceVelocity = new Vector2?(BraveMathCollege.DegreesToVector(BraveMathCollege.ClampAngle360(angle1 + (float) (2.0 * ((double) angle2 - (double) angle1)))).normalized * velocity.magnitude);
  }

  private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
  {
    if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody && (bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.projectile)
    {
      Vector2 normalized = rigidbodyCollision.OtherRigidbody.projectile.LastVelocity.normalized;
      rigidbodyCollision.MyRigidbody.Velocity += normalized * 1.5f;
      AIAnimator aiAnimator = rigidbodyCollision.MyRigidbody.aiAnimator;
      if (!(bool) (UnityEngine.Object) aiAnimator || (double) aiAnimator.CurrentClipProgress < 1.0)
        return;
      aiAnimator.PlayUntilCancelled("kick" + this.kickCount.ToString(), true);
      this.kickCount = this.kickCount % 8 + 1;
    }
    else
    {
      Vector2 velocity = rigidbodyCollision.MyRigidbody.Velocity;
      float angle1 = (-velocity).ToAngle();
      float angle2 = rigidbodyCollision.Normal.ToAngle();
      PhysicsEngine.PostSliceVelocity = new Vector2?(BraveMathCollege.DegreesToVector(BraveMathCollege.ClampAngle360(angle1 + (float) (2.0 * ((double) angle2 - (double) angle1)))).normalized * velocity.magnitude);
    }
  }

  private void HandlePostRigidbodyMotion(SpeculativeRigidbody arg1, Vector2 arg2, IntVector2 arg3)
  {
    arg1.Velocity = Vector2.MoveTowards(arg1.Velocity, Vector2.zero, 5f * BraveTime.DeltaTime);
    if (this.m_isFalling || !GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) arg1.UnitTopLeft) || !GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) arg1.UnitTopRight) || !GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) arg1.UnitBottomLeft) || !GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) arg1.UnitBottomRight))
      return;
    GameManager.Instance.Dungeon.StartCoroutine(this.HandlePitfall(arg1));
  }

  [DebuggerHidden]
  private IEnumerator HandlePitfall(SpeculativeRigidbody srb)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GetKicked.<HandlePitfall>c__Iterator0()
    {
      srb = srb,
      _this = this
    };
  }

  public void SetAnimationState(PlayerController interactor, TalkDoerLite owner)
  {
    bool flag = false;
    string animationName = "tablekick_up";
    switch (BraveMathCollege.VectorToQuadrant(interactor.CenterPosition - owner.specRigidbody.UnitCenter))
    {
      case 0:
        animationName = "tablekick_down";
        break;
      case 1:
        flag = true;
        animationName = "tablekick_right";
        break;
      case 2:
        animationName = "tablekick_up";
        break;
      case 3:
        animationName = "tablekick_right";
        break;
    }
    interactor.QueueSpecificAnimation(animationName);
  }
}
