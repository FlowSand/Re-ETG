// Decompiled with JetBrains decompiler
// Type: ProjectileData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class ProjectileData
{
  public static float FixedFallbackDamageToEnemies = 10f;
  public static float FixedEnemyDamageToBreakables = 8f;
  public float damage = 2.5f;
  public float speed = 10f;
  public float range = 10f;
  public float force = 10f;
  public float damping;
  public bool UsesCustomAccelerationCurve;
  [ShowInInspectorIf("UsesCustomAccelerationCurve", true)]
  public AnimationCurve AccelerationCurve;
  [ShowInInspectorIf("UsesCustomAccelerationCurve", true)]
  public float CustomAccelerationCurveDuration = 1f;
  [NonSerialized]
  public float IgnoreAccelCurveTime;
  public BulletScriptSelector onDestroyBulletScript;

  public ProjectileData()
  {
  }

  public ProjectileData(ProjectileData other) => this.SetAll(other);

  public void SetAll(ProjectileData other)
  {
    this.damage = other.damage;
    this.speed = other.speed;
    this.range = other.range;
    this.force = other.force;
    this.damping = other.damping;
    this.UsesCustomAccelerationCurve = other.UsesCustomAccelerationCurve;
    this.AccelerationCurve = other.AccelerationCurve;
    this.CustomAccelerationCurveDuration = other.CustomAccelerationCurveDuration;
    this.onDestroyBulletScript = other.onDestroyBulletScript.Clone();
  }
}
