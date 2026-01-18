// Decompiled with JetBrains decompiler
// Type: BounceProjModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

public class BounceProjModifier : BraveBehaviour
  {
    public int numberOfBounces = 1;
    public float chanceToDieOnBounce;
    public float percentVelocityToLoseOnBounce;
    public float damageMultiplierOnBounce = 1f;
    public bool usesAdditionalScreenShake;
    [ShowInInspectorIf("usesAdditionalScreenShake", false)]
    public ScreenShakeSettings additionalScreenShake;
    public bool useLayerLimit;
    [ShowInInspectorIf("useLayerLimit", false)]
    public CollisionLayer layerLimit;
    public Action<BounceProjModifier, SpeculativeRigidbody> OnBounceContext;
    public bool ExplodeOnEnemyBounce;
    [FormerlySerializedAs("removeBulletMlControl")]
    public bool removeBulletScriptControl = true;
    public bool suppressHitEffectsOnBounce;
    public bool onlyBounceOffTiles;
    public bool bouncesTrackEnemies;
    public float bounceTrackRadius = 5f;
    public float TrackEnemyChance = 1f;
    private AIActor m_lastSmartBounceTarget;
    private int m_cachedNumberOfBounces;
    private Vector2 m_lastBouncePos;

    public event System.Action OnBounce;

    public void OnSpawned()
    {
      this.m_cachedNumberOfBounces = this.numberOfBounces;
      this.m_lastBouncePos = Vector2.zero;
    }

    public void OnDespawned() => this.numberOfBounces = this.m_cachedNumberOfBounces;

    public Vector2 AdjustBounceVector(
      Projectile source,
      Vector2 inVec,
      SpeculativeRigidbody hitRigidbody)
    {
      Vector2 vector2_1 = inVec;
      if (this.bouncesTrackEnemies && (double) UnityEngine.Random.value < (double) this.TrackEnemyChance)
      {
        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(source.specRigidbody.UnitCenter.ToIntVector2());
        Vector2 unitCenter = source.specRigidbody.UnitCenter;
        Vector2 w = unitCenter + inVec.normalized * 50f;
        List<AIActor> activeEnemies = roomFromPosition.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        float num1 = this.bounceTrackRadius * this.bounceTrackRadius;
        AIActor aiActor = (AIActor) null;
        float num2 = float.MaxValue;
        if (activeEnemies != null)
        {
          for (int index = 0; index < activeEnemies.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) activeEnemies[index] && !((UnityEngine.Object) activeEnemies[index] == (UnityEngine.Object) this.m_lastSmartBounceTarget) && !((UnityEngine.Object) activeEnemies[index].specRigidbody == (UnityEngine.Object) hitRigidbody))
            {
              Vector2 vector2_2 = BraveMathCollege.ClosestPointOnLineSegment(activeEnemies[index].CenterPosition, unitCenter, w);
              float num3 = Vector2.SqrMagnitude(activeEnemies[index].CenterPosition - vector2_2);
              if ((double) num3 < (double) num1 && (double) num3 < (double) num2)
              {
                num2 = num3;
                aiActor = activeEnemies[index];
              }
            }
          }
        }
        this.m_lastSmartBounceTarget = aiActor;
        if ((UnityEngine.Object) aiActor != (UnityEngine.Object) null)
          vector2_1 = (aiActor.CenterPosition - unitCenter).normalized * inVec.magnitude;
      }
      return vector2_1;
    }

    public bool FarFromPreviousBounce(Vector2 pos)
    {
      return (double) Vector2.Distance(pos, this.m_lastBouncePos) > 0.25;
    }

    public void Bounce(Projectile p, Vector2 bouncePos, SpeculativeRigidbody otherRigidbody = null)
    {
      this.m_lastBouncePos = bouncePos;
      this.Bounce(p, otherRigidbody);
    }

    public void Bounce(Projectile p, SpeculativeRigidbody otherRigidbody = null)
    {
      if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) p)
        return;
      PierceProjModifier pierceProjModifier = (PierceProjModifier) null;
      if ((bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.projectile)
        pierceProjModifier = otherRigidbody.GetComponent<PierceProjModifier>();
      if ((bool) (UnityEngine.Object) pierceProjModifier && pierceProjModifier.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
        this.numberOfBounces -= 2;
      else
        --this.numberOfBounces;
      p.baseData.damage *= this.damageMultiplierOnBounce;
      if (this.usesAdditionalScreenShake)
        GameManager.Instance.MainCameraController.DoScreenShake(this.additionalScreenShake, new Vector2?(p.specRigidbody.UnitCenter));
      if (this.numberOfBounces <= 0 && (bool) (UnityEngine.Object) p.TrailRendererController)
        p.TrailRendererController.Stop();
      if (this.OnBounce != null)
        this.OnBounce();
      if (this.OnBounceContext == null)
        return;
      this.OnBounceContext(this, otherRigidbody);
    }

    public void HandleChanceToDie()
    {
      if ((double) this.chanceToDieOnBounce <= 0.0 || (double) UnityEngine.Random.value >= (double) this.chanceToDieOnBounce)
        return;
      this.numberOfBounces = 0;
    }
  }

