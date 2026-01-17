// Decompiled with JetBrains decompiler
// Type: FreezeOnDeath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class FreezeOnDeath : BraveBehaviour
{
  [CheckDirectionalAnimation(null)]
  public string deathFreezeAnim;
  [CheckDirectionalAnimation(null)]
  public string deathShatterAnim;
  [CheckDirectionalAnimation(null)]
  public string deathInstantShatterAnim;
  public GameObject shatterVfx;

  public bool IsDisintegrating { get; set; }

  public bool IsDeathFrozen { get; set; }

  public void Start()
  {
    this.healthHaver.ManualDeathHandling = true;
    this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.spriteAnimator)
      this.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DeathCompleted);
    if ((bool) (UnityEngine.Object) this.specRigidbody)
      this.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
    StaticReferenceManager.AllCorpses.Add(this.gameObject);
    base.OnDestroy();
  }

  private void OnPreDeath(Vector2 dir)
  {
    if ((bool) (UnityEngine.Object) this.aiActor && (bool) (UnityEngine.Object) this.healthHaver && this.aiActor.IsFalling)
    {
      this.healthHaver.ManualDeathHandling = false;
    }
    else
    {
      this.aiAnimator.PlayUntilCancelled(this.deathFreezeAnim, true);
      this.IsDeathFrozen = true;
      this.aiActor.IsFrozen = true;
      this.aiActor.ForceDeath(Vector2.zero, false);
      this.aiActor.ImmuneToAllEffects = true;
      this.aiActor.RemoveAllEffects(true);
      this.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
      StaticReferenceManager.AllCorpses.Add(this.gameObject);
    }
  }

  private void OnCollision(CollisionData collisionData)
  {
    if (!(bool) (UnityEngine.Object) collisionData.OtherRigidbody)
      return;
    if ((bool) (UnityEngine.Object) collisionData.OtherRigidbody.projectile)
    {
      this.DoFullDeath(this.deathShatterAnim);
    }
    else
    {
      PlayerController component = collisionData.OtherRigidbody.GetComponent<PlayerController>();
      if (!(bool) (UnityEngine.Object) component || !component.IsDodgeRolling)
        return;
      this.DoFullDeath(this.deathInstantShatterAnim);
    }
  }

  private void DeathVfxTriggered(
    tk2dSpriteAnimator sprite,
    tk2dSpriteAnimationClip clip,
    int frameNum)
  {
    if (!(clip.GetFrame(frameNum).eventInfo == "vfx"))
      return;
    SpawnManager.SpawnVFX(this.shatterVfx, (Vector3) this.specRigidbody.HitboxPixelCollider.UnitCenter, Quaternion.identity);
    this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.DeathVfxTriggered);
  }

  private void DeathCompleted(
    tk2dSpriteAnimator tk2DSpriteAnimator,
    tk2dSpriteAnimationClip tk2DSpriteAnimationClip)
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  public void HandleDisintegration()
  {
    this.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
    this.specRigidbody.enabled = false;
  }

  private void DoFullDeath(string deathAnim)
  {
    this.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
    this.specRigidbody.enabled = false;
    this.aiAnimator.PlayUntilCancelled(deathAnim, true);
    if ((bool) (UnityEngine.Object) this.shatterVfx)
      this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.DeathVfxTriggered);
    this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DeathCompleted);
    StaticReferenceManager.AllCorpses.Remove(this.gameObject);
  }
}
