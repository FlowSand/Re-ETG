// Decompiled with JetBrains decompiler
// Type: MineFlayerShellGameBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/MineFlayer/ShellGameBehavior")]
    public class MineFlayerShellGameBehavior : BasicAttackBehavior
    {
      public float MaxGoneTime = 5f;
      [InspectorCategory("Attack")]
      public BulletScriptSelector disappearBulletScript;
      [InspectorCategory("Attack")]
      public BulletScriptSelector reappearInBulletScript;
      [InspectorCategory("Visuals")]
      public string disappearAnim = "teleport_out";
      [InspectorCategory("Visuals")]
      public string reappearAnim = "teleport_in";
      [InspectorCategory("Visuals")]
      public bool requiresTransparency;
      [InspectorCategory("Visuals")]
      public MineFlayerShellGameBehavior.ShadowSupport shadowSupport;
      [InspectorCategory("Visuals")]
      [InspectorShowIf("ShowShadowAnimationNames")]
      public string shadowDisappearAnim;
      [InspectorShowIf("ShowShadowAnimationNames")]
      [InspectorCategory("Visuals")]
      public string shadowReappearAnim;
      public int enemiesToSpawn;
      [EnemyIdentifier]
      public string enemyGuid;
      private tk2dBaseSprite m_shadowSprite;
      private Shader m_cachedShader;
      private float m_timer;
      private bool m_shouldFire;
      private List<AIActor> m_spawnedActors = new List<AIActor>();
      private AIActor m_myBell;
      private bool m_correctBellHit;
      private Vector2? m_reappearPosition;
      private MineFlayerShellGameBehavior.ShellGameState m_state;

      private bool ShowShadowAnimationNames()
      {
        return this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Animate;
      }

      public override void Start()
      {
        base.Start();
        this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_timer);
      }

      public override BehaviorResult Update()
      {
        int num = (int) base.Update();
        if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
          this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          return BehaviorResult.Continue;
        this.State = MineFlayerShellGameBehavior.ShellGameState.Disappear;
        this.m_aiActor.healthHaver.minimumHealth = 1f;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.State == MineFlayerShellGameBehavior.ShellGameState.Disappear)
        {
          if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
          if (!this.m_aiAnimator.IsPlaying(this.disappearAnim))
            this.State = (double) this.MaxGoneTime <= 0.0 ? MineFlayerShellGameBehavior.ShellGameState.Reappear : MineFlayerShellGameBehavior.ShellGameState.Gone;
        }
        else if (this.State == MineFlayerShellGameBehavior.ShellGameState.Gone)
        {
          if ((double) this.m_timer <= 0.0)
            this.State = MineFlayerShellGameBehavior.ShellGameState.Reappear;
        }
        else if (this.State == MineFlayerShellGameBehavior.ShellGameState.Reappear)
        {
          if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
          if ((bool) (UnityEngine.Object) this.m_aiShooter)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, "MeduziUnderwaterBehavior");
          if (this.m_reappearPosition.HasValue)
          {
            this.m_aiActor.specRigidbody.CollideWithTileMap = false;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_aiActor.specRigidbody);
            Vector2 vector2 = this.m_reappearPosition.Value - this.m_aiActor.specRigidbody.UnitBottomLeft;
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = vector2 / (this.m_aiAnimator.CurrentClipLength * (1f - this.m_aiAnimator.CurrentClipProgress));
          }
          if (!this.m_aiAnimator.IsPlaying(this.reappearAnim))
          {
            this.State = MineFlayerShellGameBehavior.ShellGameState.None;
            return ContinuousBehaviorResult.Finished;
          }
        }
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.m_aiActor.healthHaver.minimumHealth = 0.0f;
        if (this.requiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
        {
          this.m_aiActor.sprite.usesOverrideMaterial = false;
          this.m_aiActor.renderer.material.shader = this.m_cachedShader;
          this.m_cachedShader = (Shader) null;
        }
        this.m_aiActor.sprite.renderer.enabled = true;
        if ((bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
          this.m_aiActor.knockbackDoer.SetImmobile(false, "teleport");
        this.m_aiActor.specRigidbody.CollideWithOthers = true;
        this.m_aiActor.IsGone = false;
        if ((bool) (UnityEngine.Object) this.m_aiShooter)
          this.m_aiShooter.ToggleGunAndHandRenderers(true, "MeduziUnderwaterBehavior");
        this.m_aiAnimator.EndAnimationIf(this.disappearAnim);
        this.m_aiAnimator.EndAnimationIf(this.reappearAnim);
        if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Fade)
          this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
        else if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Animate)
        {
          tk2dSpriteAnimationClip clipByName = this.m_shadowSprite.spriteAnimator.GetClipByName(this.shadowReappearAnim);
          this.m_shadowSprite.spriteAnimator.Play(clipByName, (float) (clipByName.frames.Length - 1), clipByName.fps);
        }
        this.m_spawnedActors.Clear();
        this.m_correctBellHit = false;
        SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
        if (this.m_reappearPosition.HasValue)
        {
          this.m_aiActor.specRigidbody.CollideWithTileMap = true;
          this.m_aiActor.transform.position += (Vector3) (this.m_reappearPosition.Value - this.m_aiActor.specRigidbody.UnitBottomLeft);
          this.m_aiActor.specRigidbody.Reinitialize();
          this.m_aiActor.BehaviorOverridesVelocity = false;
          this.m_reappearPosition = new Vector2?();
        }
        this.m_state = MineFlayerShellGameBehavior.ShellGameState.None;
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
      }

      public override bool IsOverridable() => false;

      public void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (this.m_shouldFire && clip.GetFrame(frame).eventInfo == "fire")
        {
          if (this.State == MineFlayerShellGameBehavior.ShellGameState.Reappear)
            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.reappearInBulletScript);
          else if (this.State == MineFlayerShellGameBehavior.ShellGameState.Disappear)
            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.disappearBulletScript);
          this.m_shouldFire = false;
        }
        else if (this.State == MineFlayerShellGameBehavior.ShellGameState.Disappear && clip.GetFrame(frame).eventInfo == "collider_off")
        {
          this.m_aiActor.specRigidbody.CollideWithOthers = false;
          this.m_aiActor.IsGone = true;
        }
        else
        {
          if (this.State != MineFlayerShellGameBehavior.ShellGameState.Reappear || !(clip.GetFrame(frame).eventInfo == "collider_on"))
            return;
          this.m_aiActor.specRigidbody.CollideWithOthers = true;
          this.m_aiActor.IsGone = false;
        }
      }

      private void OnMyBellDeath(Vector2 obj)
      {
        if (this.State == MineFlayerShellGameBehavior.ShellGameState.Reappear)
          return;
        this.m_correctBellHit = true;
        this.State = MineFlayerShellGameBehavior.ShellGameState.Reappear;
      }

      private MineFlayerShellGameBehavior.ShellGameState State
      {
        get => this.m_state;
        set
        {
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      private void BeginState(MineFlayerShellGameBehavior.ShellGameState state)
      {
        switch (state)
        {
          case MineFlayerShellGameBehavior.ShellGameState.Disappear:
            if (this.disappearBulletScript != null && !this.disappearBulletScript.IsNull)
              this.m_shouldFire = true;
            if (this.requiresTransparency)
            {
              this.m_cachedShader = this.m_aiActor.renderer.material.shader;
              this.m_aiActor.sprite.usesOverrideMaterial = true;
              this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            }
            this.m_aiAnimator.PlayUntilCancelled(this.disappearAnim, true);
            if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Animate)
              this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowDisappearAnim, this.m_aiAnimator.CurrentClipLength);
            this.m_aiActor.ClearPath();
            if (!(bool) (UnityEngine.Object) this.m_aiShooter)
              break;
            this.m_aiShooter.ToggleGunAndHandRenderers(false, "MeduziUnderwaterBehavior");
            break;
          case MineFlayerShellGameBehavior.ShellGameState.Gone:
            this.m_timer = this.MaxGoneTime;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            this.m_aiActor.IsGone = true;
            this.m_aiActor.sprite.renderer.enabled = false;
            Vector2 position = this.m_aiActor.specRigidbody.UnitCenter + Vector2.right;
            for (int index = 0; index < this.enemiesToSpawn; ++index)
              this.m_spawnedActors.Add(AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.enemyGuid), position, this.m_aiActor.ParentRoom, true));
            this.m_myBell = BraveUtility.RandomElement<AIActor>(this.m_spawnedActors);
            this.m_myBell.healthHaver.OnPreDeath += new Action<Vector2>(this.OnMyBellDeath);
            this.m_myBell.OnCorpseVFX.type = VFXPoolType.None;
            this.m_myBell.healthHaver.spawnBulletScript = false;
            break;
          case MineFlayerShellGameBehavior.ShellGameState.Reappear:
            if ((bool) (UnityEngine.Object) this.m_myBell)
            {
              this.m_aiActor.specRigidbody.AlignWithRigidbodyBottomCenter(this.m_myBell.specRigidbody, new IntVector2?(new IntVector2(-6, -2)));
              if (PhysicsEngine.Instance.OverlapCast(this.m_aiActor.specRigidbody))
                this.DoReposition();
              else
                this.m_reappearPosition = new Vector2?();
            }
            for (int index = 0; index < this.m_spawnedActors.Count; ++index)
            {
              AIActor spawnedActor = this.m_spawnedActors[index];
              if ((bool) (UnityEngine.Object) spawnedActor && (bool) (UnityEngine.Object) spawnedActor.healthHaver && spawnedActor.healthHaver.IsAlive)
              {
                if (this.m_correctBellHit)
                  spawnedActor.healthHaver.spawnBulletScript = false;
                spawnedActor.healthHaver.ApplyDamage(1E+10f, Vector2.zero, "Bell Death", damageCategory: DamageCategory.Unstoppable);
              }
            }
            if (this.reappearInBulletScript != null && !this.reappearInBulletScript.IsNull)
              this.m_shouldFire = true;
            this.m_aiAnimator.PlayUntilFinished(this.reappearAnim, true);
            if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Animate)
              this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowReappearAnim, this.m_aiAnimator.CurrentClipLength);
            this.m_shadowSprite.renderer.enabled = true;
            this.m_aiActor.sprite.renderer.enabled = true;
            if ((bool) (UnityEngine.Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(false, "MeduziUnderwaterBehavior");
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
            break;
        }
      }

      private void EndState(MineFlayerShellGameBehavior.ShellGameState state)
      {
        switch (state)
        {
          case MineFlayerShellGameBehavior.ShellGameState.Disappear:
            this.m_shadowSprite.renderer.enabled = false;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
            if (this.disappearBulletScript == null || this.disappearBulletScript.IsNull || !this.m_shouldFire)
              break;
            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.disappearBulletScript);
            this.m_shouldFire = false;
            break;
          case MineFlayerShellGameBehavior.ShellGameState.Gone:
            this.m_aiActor.BehaviorOverridesVelocity = false;
            break;
          case MineFlayerShellGameBehavior.ShellGameState.Reappear:
            if (this.requiresTransparency)
            {
              this.m_aiActor.sprite.usesOverrideMaterial = false;
              this.m_aiActor.renderer.material.shader = this.m_cachedShader;
            }
            if (this.shadowSupport == MineFlayerShellGameBehavior.ShadowSupport.Fade)
              this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
            this.m_aiActor.specRigidbody.CollideWithOthers = true;
            this.m_aiActor.IsGone = false;
            if ((bool) (UnityEngine.Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(true, "MeduziUnderwaterBehavior");
            if (this.reappearInBulletScript != null && !this.reappearInBulletScript.IsNull && this.m_shouldFire)
            {
              SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.reappearInBulletScript);
              this.m_shouldFire = false;
            }
            if (!this.m_reappearPosition.HasValue)
              break;
            this.m_aiActor.specRigidbody.CollideWithTileMap = true;
            this.m_aiActor.transform.position += (Vector3) (this.m_reappearPosition.Value - this.m_aiActor.specRigidbody.UnitBottomLeft);
            this.m_aiActor.specRigidbody.Reinitialize();
            this.m_aiActor.BehaviorOverridesVelocity = false;
            this.m_reappearPosition = new Vector2?();
            break;
        }
      }

      private void DoReposition()
      {
        IntVector2? nearestAvailableCell = this.m_aiActor.ParentRoom.GetNearestAvailableCell(this.m_aiActor.specRigidbody.UnitBottomLeft, new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: (CellValidator) (c =>
        {
          for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
          {
            for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
            {
              if (GameManager.Instance.Dungeon.data.isWall(c.x + index1, c.y + index2) || GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
                return false;
            }
          }
          return true;
        }));
        if (nearestAvailableCell.HasValue)
          this.m_reappearPosition = new Vector2?(Pathfinder.GetClearanceOffset(nearestAvailableCell.Value, this.m_aiActor.Clearance).WithY((float) nearestAvailableCell.Value.y) - new Vector2(this.m_aiActor.specRigidbody.UnitDimensions.x / 2f, 0.0f));
        else
          this.m_reappearPosition = new Vector2?();
      }

      public enum ShadowSupport
      {
        None,
        Fade,
        Animate,
      }

      private enum ShellGameState
      {
        None,
        Disappear,
        Gone,
        Reappear,
      }
    }

}
