// Decompiled with JetBrains decompiler
// Type: HomingModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class HomingModifier : BraveBehaviour
{
  public float HomingRadius = 2f;
  public float AngularVelocity = 180f;
  protected Projectile m_projectile;

  private void Start()
  {
    if (!(bool) (UnityEngine.Object) this.m_projectile)
      this.m_projectile = this.GetComponent<Projectile>();
    this.m_projectile.ModifyVelocity += new Func<Vector2, Vector2>(this.ModifyVelocity);
  }

  public void AssignProjectile(Projectile source) => this.m_projectile = source;

  private Vector2 ModifyVelocity(Vector2 inVel)
  {
    Vector2 vector2_1 = inVel;
    List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_projectile.LastPosition.IntXY(VectorConversions.Floor)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    if (activeEnemies == null || activeEnemies.Count == 0)
      return inVel;
    float f = float.MaxValue;
    Vector2 vector = Vector2.zero;
    AIActor aiActor1 = (AIActor) null;
    Vector2 vector2_2 = !(bool) (UnityEngine.Object) this.sprite ? this.transform.position.XY() : this.sprite.WorldCenter;
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      AIActor aiActor2 = activeEnemies[index];
      if ((bool) (UnityEngine.Object) aiActor2 && aiActor2.IsWorthShootingAt && !aiActor2.IsGone)
      {
        Vector2 vector2_3 = aiActor2.CenterPosition - vector2_2;
        float sqrMagnitude = vector2_3.sqrMagnitude;
        if ((double) sqrMagnitude < (double) f)
        {
          vector = vector2_3;
          f = sqrMagnitude;
          aiActor1 = aiActor2;
        }
      }
    }
    float num1 = Mathf.Sqrt(f);
    if ((double) num1 < (double) this.HomingRadius && (UnityEngine.Object) aiActor1 != (UnityEngine.Object) null)
    {
      float num2 = (float) (1.0 - (double) num1 / (double) this.HomingRadius);
      float angle1 = vector.ToAngle();
      float angle2 = inVel.ToAngle();
      float maxDelta = this.AngularVelocity * num2 * this.m_projectile.LocalDeltaTime;
      float num3 = Mathf.MoveTowardsAngle(angle2, angle1, maxDelta);
      if (this.m_projectile is HelixProjectile)
      {
        (this.m_projectile as HelixProjectile).AdjustRightVector(num3 - angle2);
      }
      else
      {
        if (this.m_projectile.shouldRotate)
          this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num3);
        vector2_1 = BraveMathCollege.DegreesToVector(num3, inVel.magnitude);
      }
      if (this.m_projectile.OverrideMotionModule != null)
        this.m_projectile.OverrideMotionModule.AdjustRightVector(num3 - angle2);
    }
    return vector2_1 == Vector2.zero || float.IsNaN(vector2_1.x) || float.IsNaN(vector2_1.y) ? inVel : vector2_1;
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.m_projectile)
      this.m_projectile.ModifyVelocity -= new Func<Vector2, Vector2>(this.ModifyVelocity);
    base.OnDestroy();
  }
}
