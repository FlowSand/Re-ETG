// Decompiled with JetBrains decompiler
// Type: AffectEnemiesInRoomItem
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
    public abstract class AffectEnemiesInRoomItem : PlayerItem
    {
      public float EffectTime;
      public GameObject OnUserEffectVFX;
      public GameObject OnTargetEffectVFX;
      public float AmbientVFXTime;
      public GameObject AmbientVFX;
      public float minTimeBetweenAmbientVFX = 0.1f;
      public bool FlashScreen;
      public bool AffectsBosses;
      private float m_ambientTimer;

      protected override void DoEffect(PlayerController user)
      {
        List<AIActor> activeEnemies = user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if ((Object) this.OnUserEffectVFX != (Object) null)
          SpawnManager.SpawnVFX(this.OnUserEffectVFX, (Vector3) user.CenterPosition, Quaternion.identity, false);
        if (this.FlashScreen)
          Pixelator.Instance.FadeToColor(0.1f, Color.white, true, 0.1f);
        if (activeEnemies == null)
          return;
        if ((double) this.EffectTime <= 0.0)
        {
          for (int index = 0; index < activeEnemies.Count; ++index)
          {
            AIActor target = activeEnemies[index];
            if (this.AffectsBosses || !target.healthHaver.IsBoss)
            {
              this.AffectEnemy(target);
              if ((Object) this.OnTargetEffectVFX != (Object) null)
                SpawnManager.SpawnVFX(this.OnTargetEffectVFX, (Vector3) target.CenterPosition, Quaternion.identity, false);
            }
          }
          if ((double) this.AmbientVFXTime <= 0.0 || !((Object) this.AmbientVFX != (Object) null))
            return;
          user.StartCoroutine(this.HandleAmbientSpawnTime(user.CenterPosition, this.AmbientVFXTime, 10f));
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
      protected IEnumerator HandleAmbientSpawnTime(
        Vector2 centerPoint,
        float remainingTime,
        float maxEffectRadius)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AffectEnemiesInRoomItem.<HandleAmbientSpawnTime>c__Iterator0()
        {
          remainingTime = remainingTime,
          centerPoint = centerPoint,
          maxEffectRadius = maxEffectRadius,
          $this = this
        };
      }

      [DebuggerHidden]
      protected IEnumerator ProcessEffectOverTime(Vector2 centerPoint, List<AIActor> enemiesInRoom)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AffectEnemiesInRoomItem.<ProcessEffectOverTime>c__Iterator1()
        {
          enemiesInRoom = enemiesInRoom,
          centerPoint = centerPoint,
          $this = this
        };
      }

      protected abstract void AffectEnemy(AIActor target);

      protected override void OnDestroy() => base.OnDestroy();
    }

}
