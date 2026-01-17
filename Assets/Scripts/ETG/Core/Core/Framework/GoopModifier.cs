// Decompiled with JetBrains decompiler
// Type: GoopModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class GoopModifier : BraveBehaviour
    {
      public GoopDefinition goopDefinition;
      public bool IsSynergyContingent;
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public bool SpawnGoopInFlight;
      [ShowInInspectorIf("SpawnGoopInFlight", true)]
      public float InFlightSpawnFrequency = 0.05f;
      [ShowInInspectorIf("SpawnGoopInFlight", true)]
      public float InFlightSpawnRadius = 1f;
      public bool SpawnGoopOnCollision;
      [ShowInInspectorIf("SpawnGoopOnCollision", true)]
      public bool OnlyGoopOnEnemyCollision;
      [ShowInInspectorIf("SpawnGoopOnCollision", true)]
      public float CollisionSpawnRadius = 3f;
      public bool SpawnAtBeamEnd;
      [ShowInInspectorIf("SpawnAtBeamEnd", true)]
      public float BeamEndRadius = 1f;
      public Vector2 spawnOffset = new Vector2(0.0f, -0.5f);
      public bool UsesInitialDelay;
      public float InitialDelay = 0.25f;
      private float m_totalElapsed;
      [NonSerialized]
      public bool SynergyViable;
      private DeadlyDeadlyGoopManager m_manager;
      private float elapsed;

      public DeadlyDeadlyGoopManager Manager
      {
        get
        {
          if ((UnityEngine.Object) this.m_manager == (UnityEngine.Object) null)
            this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
          return this.m_manager;
        }
      }

      public void Start()
      {
        if (this.IsSynergyContingent)
        {
          if ((bool) (UnityEngine.Object) this.projectile && (bool) (UnityEngine.Object) this.projectile.PossibleSourceGun && this.projectile.PossibleSourceGun.CurrentOwner is PlayerController)
          {
            if (!(this.projectile.PossibleSourceGun.CurrentOwner as PlayerController).HasActiveBonusSynergy(this.RequiredSynergy))
            {
              this.enabled = false;
              return;
            }
            this.SynergyViable = true;
          }
          else
          {
            this.enabled = false;
            return;
          }
        }
        if (!(bool) (UnityEngine.Object) this.GetComponent<BeamController>())
          return;
        this.enabled = false;
      }

      public void Update()
      {
        if ((UnityEngine.Object) this.m_manager == (UnityEngine.Object) null)
          this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
        if (this.IsSynergyContingent && (bool) (UnityEngine.Object) this.projectile && this.projectile.OverrideMotionModule != null && this.projectile.OverrideMotionModule is OrbitProjectileMotionModule)
        {
          this.enabled = false;
        }
        else
        {
          if (!this.SpawnGoopInFlight)
            return;
          this.elapsed += BraveTime.DeltaTime;
          this.m_totalElapsed += BraveTime.DeltaTime;
          if (this.UsesInitialDelay && (double) this.m_totalElapsed <= (double) this.InitialDelay || (double) this.elapsed < (double) this.InFlightSpawnFrequency)
            return;
          this.elapsed -= this.InFlightSpawnFrequency;
          Vector2 vector2 = this.projectile.sprite.WorldCenter + this.spawnOffset - this.projectile.transform.position.XY();
          this.m_manager.AddGoopLine(this.projectile.sprite.WorldCenter + this.spawnOffset, this.projectile.LastPosition.XY() + vector2, this.InFlightSpawnRadius);
          if (!this.goopDefinition.CanBeFrozen || (this.projectile.damageTypes | CoreDamageTypes.Ice) != this.projectile.damageTypes)
            return;
          this.Manager.FreezeGoopCircle(this.projectile.sprite.WorldCenter, this.InFlightSpawnRadius);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void SpawnCollisionGoop(Vector2 pos)
      {
        if (this.IsSynergyContingent && !this.SynergyViable || !this.SpawnGoopOnCollision)
          return;
        this.Manager.TimedAddGoopCircle(pos, this.CollisionSpawnRadius);
        if (!this.goopDefinition.CanBeFrozen || (this.projectile.damageTypes | CoreDamageTypes.Ice) != this.projectile.damageTypes)
          return;
        this.Manager.FreezeGoopCircle(pos, this.CollisionSpawnRadius);
      }

      public void SpawnCollisionGoop(CollisionData lcr)
      {
        if (this.IsSynergyContingent && !this.SynergyViable || !this.SpawnGoopOnCollision)
          return;
        this.Manager.TimedAddGoopCircle(lcr.Contact, this.CollisionSpawnRadius);
        if (!this.goopDefinition.CanBeFrozen || (this.projectile.damageTypes | CoreDamageTypes.Ice) != this.projectile.damageTypes)
          return;
        this.Manager.FreezeGoopCircle(lcr.Contact, this.CollisionSpawnRadius);
      }
    }

}
