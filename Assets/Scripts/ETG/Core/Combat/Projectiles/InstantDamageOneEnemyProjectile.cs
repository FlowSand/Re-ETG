using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class InstantDamageOneEnemyProjectile : Projectile
  {
    public bool DoesWhiteFlash;
    public bool DoesCameraFlash;
    public bool DoesStickyFriction;
    public float StickyFrictionDuration = 0.6f;
    public bool DoesAmbientVFX;
    public float AmbientVFXTime;
    public GameObject AmbientVFX;
    public float minTimeBetweenAmbientVFX = 0.1f;
    public GameObject DamagedEnemyVFX;
    private float m_ambientTimer;

    protected override void Move()
    {
      if (this.DoesWhiteFlash)
        Pixelator.Instance.FadeToColor(0.1f, Color.white.WithAlpha(0.25f), true, 0.1f);
      if (this.DoesCameraFlash)
      {
        StickyFrictionManager.Instance.RegisterCustomStickyFriction(0.125f, 0.0f, false);
        Pixelator.Instance.TimedFreezeFrame(0.25f, 0.125f);
      }
      if (this.DoesAmbientVFX && (double) this.AmbientVFXTime > 0.0 && (Object) this.AmbientVFX != (Object) null)
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleAmbientSpawnTime((Vector2) this.transform.position, this.AmbientVFXTime));
      if (this.DoesStickyFriction)
        StickyFrictionManager.Instance.RegisterCustomStickyFriction(this.StickyFrictionDuration, 0.0f, true);
      RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
      if (absoluteRoom != null)
      {
        List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies != null)
        {
          AIActor a = (AIActor) null;
          float num1 = float.MaxValue;
          Vector2 b = this.Owner.CenterPosition;
          if (this.Owner is PlayerController)
            b = (this.Owner as PlayerController).unadjustedAimPoint.XY();
          for (int index = 0; index < activeEnemies.Count; ++index)
          {
            if ((bool) (Object) activeEnemies[index] && activeEnemies[index].IsNormalEnemy && (bool) (Object) activeEnemies[index].healthHaver && activeEnemies[index].isActiveAndEnabled)
            {
              float num2 = Vector2.Distance(activeEnemies[index].CenterPosition, b);
              if ((double) num2 < (double) num1)
              {
                num1 = num2;
                a = activeEnemies[index];
              }
            }
          }
          if ((bool) (Object) a)
            this.ProcessEnemy(a, 0.0f);
        }
      }
      this.DieInAir();
    }

    protected void HandleAmbientVFXSpawn(Vector2 centerPoint, float radius)
    {
      if ((Object) this.AmbientVFX == (Object) null)
        return;
      bool flag = false;
      this.m_ambientTimer -= BraveTime.DeltaTime;
      if ((double) this.m_ambientTimer <= 0.0)
      {
        flag = true;
        this.m_ambientTimer = this.minTimeBetweenAmbientVFX;
      }
      if (!flag)
        return;
      SpawnManager.SpawnVFX(this.AmbientVFX, (Vector3) (centerPoint + Random.insideUnitCircle * radius), Quaternion.identity);
    }

    [DebuggerHidden]
    protected IEnumerator HandleAmbientSpawnTime(Vector2 centerPoint, float remainingTime)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InstantDamageOneEnemyProjectile__HandleAmbientSpawnTimec__Iterator0()
      {
        remainingTime = remainingTime,
        centerPoint = centerPoint,
        _this = this
      };
    }

    public void ProcessEnemy(AIActor a, float b)
    {
      if (!(bool) (Object) a || !a.IsNormalEnemy || !(bool) (Object) a.healthHaver)
        return;
      if ((bool) (Object) this.Owner)
      {
        a.healthHaver.ApplyDamage(this.ModifiedDamage, Vector2.zero, this.OwnerName, this.damageTypes);
        this.LastVelocity = (a.CenterPosition - this.Owner.CenterPosition).normalized;
        this.HandleKnockback(a.specRigidbody, this.Owner as PlayerController, true);
      }
      else
        a.healthHaver.ApplyDamage(this.ModifiedDamage, Vector2.zero, "projectile", this.damageTypes);
      if (!((Object) this.DamagedEnemyVFX != (Object) null))
        return;
      a.PlayEffectOnActor(this.DamagedEnemyVFX, Vector3.zero, false, true);
    }
  }

