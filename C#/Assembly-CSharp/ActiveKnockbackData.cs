// Decompiled with JetBrains decompiler
// Type: ActiveKnockbackData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ActiveKnockbackData
{
  public Vector2 initialKnockback;
  public Vector2 knockback;
  public float elapsedTime;
  public float curveTime;
  public AnimationCurve curveFalloff;
  public GameObject sourceObject;
  public bool immutable;

  public ActiveKnockbackData(Vector2 k, float t, bool i)
  {
    this.knockback = k;
    this.initialKnockback = k;
    this.curveTime = t;
    this.immutable = i;
  }

  public ActiveKnockbackData(Vector2 k, AnimationCurve curve, float t, bool i)
  {
    this.knockback = k;
    this.initialKnockback = k;
    this.curveFalloff = curve;
    this.curveTime = t;
    this.immutable = i;
  }
}
