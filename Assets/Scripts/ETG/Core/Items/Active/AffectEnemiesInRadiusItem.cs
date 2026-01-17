// Decompiled with JetBrains decompiler
// Type: AffectEnemiesInRadiusItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public abstract class AffectEnemiesInRadiusItem : PlayerItem
    {
      public float EffectRadius = 10f;
      public float EffectTime;
      public Vector2 OnUserEffectOffset = (Vector2) Vector3.zero;
      public bool OnUserEffectAttached;
      public GameObject OnUserEffectVFX;
      public GameObject OnTargetEffectVFX;
      public string AudioEvent;
      public float AmbientVFXTime;
      public GameObject AmbientVFX;
      public float minTimeBetweenAmbientVFX = 0.1f;
      public bool FlashScreen;
      public bool ShakeScreen;
      [ShowInInspectorIf("ShakeScreen", false)]
      public ScreenShakeSettings ScreenShakeData;
      public bool DoEffectDistortionWave;
      private float m_ambientTimer;

      protected override void DoEffect(PlayerController user)
      {
        List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if ((Object) this.OnUserEffectVFX != (Object) null)
        {
          if (this.OnUserEffectAttached)
            user.PlayEffectOnActor(this.OnUserEffectVFX, (Vector3) this.OnUserEffectOffset);
          else
            SpawnManager.SpawnVFX(this.OnUserEffectVFX, (Vector3) (user.CenterPosition + this.OnUserEffectOffset), Quaternion.identity, false);
        }
        if (this.ShakeScreen)
          GameManager.Instance.MainCameraController.DoScreenShake(this.ScreenShakeData, new Vector2?());
        if (this.FlashScreen)
          Pixelator.Instance.FadeToColor(0.1f, Color.white, true, 0.1f);
        if (this.DoEffectDistortionWave)
          Exploder.DoDistortionWave(user.CenterPosition, 0.4f, 0.15f, this.EffectRadius, 0.4f);
        if (!string.IsNullOrEmpty(this.AudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.AudioEvent, this.gameObject);
        }
        if ((double) this.EffectTime <= 0.0)
        {
          if (activeEnemies != null)
          {
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              AIActor target = activeEnemies[index];
              if (target.IsNormalEnemy && (double) Vector2.Distance(user.CenterPosition, target.CenterPosition) <= (double) this.EffectRadius)
              {
                this.AffectEnemy(target);
                if ((Object) this.OnTargetEffectVFX != (Object) null)
                  SpawnManager.SpawnVFX(this.OnTargetEffectVFX, (Vector3) target.CenterPosition, Quaternion.identity, false);
              }
            }
            if ((double) this.AmbientVFXTime > 0.0 && (Object) this.AmbientVFX != (Object) null)
              user.StartCoroutine(this.HandleAmbientSpawnTime(user.CenterPosition, this.AmbientVFXTime));
          }
          List<ProjectileTrapController> allProjectileTraps = StaticReferenceManager.AllProjectileTraps;
          for (int index = 0; index < allProjectileTraps.Count; ++index)
          {
            ProjectileTrapController target = allProjectileTraps[index];
            if ((bool) (Object) target && target.isActiveAndEnabled && (double) Vector2.Distance(user.CenterPosition, (Vector2) target.shootPoint.position) <= (double) this.EffectRadius)
            {
              this.AffectProjectileTrap(target);
              if ((Object) this.OnTargetEffectVFX != (Object) null)
                SpawnManager.SpawnVFX(this.OnTargetEffectVFX, target.shootPoint.position, Quaternion.identity, false);
            }
          }
          List<ForgeHammerController> allForgeHammers = StaticReferenceManager.AllForgeHammers;
          for (int index = 0; index < allForgeHammers.Count; ++index)
          {
            ForgeHammerController target = allForgeHammers[index];
            if ((bool) (Object) target && target.isActiveAndEnabled && (double) Vector2.Distance(user.CenterPosition, target.sprite.WorldCenter) <= (double) this.EffectRadius)
              this.AffectForgeHammer(target);
          }
          List<BaseShopController> allShops = StaticReferenceManager.AllShops;
          for (int index = 0; index < allShops.Count; ++index)
          {
            BaseShopController target = allShops[index];
            if ((double) Vector2.Distance(user.CenterPosition, target.CenterPosition) <= (double) this.EffectRadius)
            {
              this.AffectShop(target);
              if ((Object) this.OnTargetEffectVFX != (Object) null)
                SpawnManager.SpawnVFX(this.OnTargetEffectVFX, (Vector3) target.CenterPosition, Quaternion.identity, false);
            }
          }
          List<MajorBreakable> allMajorBreakables = StaticReferenceManager.AllMajorBreakables;
          for (int index = 0; index < allMajorBreakables.Count; ++index)
          {
            MajorBreakable majorBreakable = allMajorBreakables[index];
            if ((bool) (Object) majorBreakable.specRigidbody && majorBreakable.specRigidbody.PrimaryPixelCollider != null && (double) Vector2.Distance(user.CenterPosition, majorBreakable.specRigidbody.UnitCenter) <= (double) this.EffectRadius)
            {
              this.AffectMajorBreakable(majorBreakable);
              if ((Object) this.OnTargetEffectVFX != (Object) null)
                SpawnManager.SpawnVFX(this.OnTargetEffectVFX, (Vector3) majorBreakable.specRigidbody.UnitCenter, Quaternion.identity, false);
            }
          }
        }
        else
          user.StartCoroutine(this.ProcessEffectOverTime(user.CenterPosition, activeEnemies));
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
        return (IEnumerator) new AffectEnemiesInRadiusItem.\u003CHandleAmbientSpawnTime\u003Ec__Iterator0()
        {
          remainingTime = remainingTime,
          centerPoint = centerPoint,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      protected IEnumerator ProcessEffectOverTime(Vector2 centerPoint, List<AIActor> enemiesInRoom)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AffectEnemiesInRadiusItem.\u003CProcessEffectOverTime\u003Ec__Iterator1()
        {
          enemiesInRoom = enemiesInRoom,
          centerPoint = centerPoint,
          \u0024this = this
        };
      }

      protected abstract void AffectEnemy(AIActor target);

      protected virtual void AffectProjectileTrap(ProjectileTrapController target)
      {
      }

      protected virtual void AffectShop(BaseShopController target)
      {
      }

      protected virtual void AffectForgeHammer(ForgeHammerController target)
      {
      }

      protected virtual void AffectMajorBreakable(MajorBreakable majorBreakable)
      {
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
