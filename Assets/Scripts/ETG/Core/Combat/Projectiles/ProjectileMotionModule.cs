// Decompiled with JetBrains decompiler
// Type: ProjectileMotionModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

  public abstract class ProjectileMotionModule
  {
    public abstract void UpdateDataOnBounce(float angleDiff);

    public abstract void Move(
      Projectile source,
      Transform projectileTransform,
      tk2dBaseSprite projectileSprite,
      SpeculativeRigidbody specRigidbody,
      ref float m_timeElapsed,
      ref Vector2 m_currentDirection,
      bool Inverted,
      bool shouldRotate);

    public virtual void AdjustRightVector(float angleDiff)
    {
    }

    public virtual void SentInDirection(
      ProjectileData baseData,
      Transform projectileTransform,
      tk2dBaseSprite projectileSprite,
      SpeculativeRigidbody specRigidbody,
      ref float m_timeElapsed,
      ref Vector2 m_currentDirection,
      bool shouldRotate,
      Vector2 dirVec,
      bool resetDistance,
      bool updateRotation)
    {
    }
  }

