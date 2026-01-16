// Decompiled with JetBrains decompiler
// Type: HeroSwordItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class HeroSwordItem : PlayerItem
{
  public float Damage = 20f;
  public float MaxHealthDamage = 30f;
  public float DamageLength = 1.25f;
  public float MaxHealthDamageLength = 2.5f;
  private float SwingDuration = 0.5f;
  public VFXPool NormalSwordVFX;
  public VFXPool MaxHealthSwordVFX;

  protected override void DoEffect(PlayerController user)
  {
    Vector2 vector2 = user.unadjustedAimPoint.XY() - user.CenterPosition;
    float zRotation = BraveMathCollege.Atan2Degrees(vector2);
    float rayDamage = this.Damage;
    float rayLength = this.DamageLength;
    if ((double) user.healthHaver.GetCurrentHealthPercentage() >= 1.0)
    {
      rayDamage = this.MaxHealthDamage;
      rayLength = this.MaxHealthDamageLength;
      this.MaxHealthSwordVFX.SpawnAtPosition((Vector3) user.CenterPosition, zRotation, user.transform, heightOffGround: new float?(1f), spriteParent: user.sprite);
    }
    else
      this.NormalSwordVFX.SpawnAtPosition((Vector3) user.CenterPosition, zRotation, user.transform, heightOffGround: new float?(1f), spriteParent: user.sprite);
    user.StartCoroutine(this.HandleSwing(user, vector2, rayDamage, rayLength));
  }

  [DebuggerHidden]
  private IEnumerator HandleSwing(
    PlayerController user,
    Vector2 aimVec,
    float rayDamage,
    float rayLength)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new HeroSwordItem.\u003CHandleSwing\u003Ec__Iterator0()
    {
      user = user,
      aimVec = aimVec,
      rayLength = rayLength,
      rayDamage = rayDamage,
      \u0024this = this
    };
  }

  protected SpeculativeRigidbody IterativeRaycast(
    Vector2 rayOrigin,
    Vector2 rayDirection,
    float rayDistance,
    int collisionMask,
    SpeculativeRigidbody ignoreRigidbody)
  {
    int num = 0;
    RaycastResult result;
    while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out result, rayMask: collisionMask, sourceLayer: new CollisionLayer?(CollisionLayer.Projectile), ignoreRigidbody: ignoreRigidbody))
    {
      ++num;
      SpeculativeRigidbody speculativeRigidbody = result.SpeculativeRigidbody;
      if (num < 3 && (Object) speculativeRigidbody != (Object) null)
      {
        MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
        if ((Object) component != (Object) null)
        {
          component.Break(rayDirection.normalized * 3f);
          RaycastResult.Pool.Free(ref result);
          continue;
        }
      }
      RaycastResult.Pool.Free(ref result);
      return speculativeRigidbody;
    }
    return (SpeculativeRigidbody) null;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
