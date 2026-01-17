// Decompiled with JetBrains decompiler
// Type: DisplacedImageController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class DisplacedImageController : BraveBehaviour
    {
      public float DamagePercentForMaxFade = 0.7f;
      public float UnfadeDelayTime = 1f;
      public float FadeRecoveryTime = 1f;
      private bool m_initialized;
      private AIActor m_host;
      private float m_lastHostHealth;
      private float m_lastImageHealth;
      private float m_fade = -1f;
      private float m_unfadeDelayTimer;

      public float Fade
      {
        get => this.m_fade;
        set
        {
          if ((double) this.m_fade == (double) value)
            return;
          this.m_fade = value;
          this.OnFadeChange(this.aiActor, Mathf.Clamp(this.m_fade, 0.0f, 0.85f), false);
          this.OnFadeChange(this.m_host, 1f - this.m_fade, true);
        }
      }

      public void Update()
      {
        if ((double) this.m_unfadeDelayTimer > 0.0)
        {
          this.m_unfadeDelayTimer = Mathf.Max(0.0f, this.m_unfadeDelayTimer - BraveTime.DeltaTime);
        }
        else
        {
          if ((double) this.Fade <= 0.0)
            return;
          this.Fade -= BraveTime.DeltaTime / this.FadeRecoveryTime;
        }
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        this.ClearHost();
        if (!(bool) (UnityEngine.Object) this.healthHaver)
          return;
        this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnImagePreDeath);
        this.healthHaver.OnDeath -= new Action<Vector2>(this.OnImageDeath);
      }

      public void Init()
      {
        if (this.m_initialized)
          return;
        this.aiActor.CanDropCurrency = false;
        this.aiActor.CanDropItems = false;
        this.aiActor.CollisionDamage = 0.0f;
        this.aiActor.MovementSpeed = 6f;
        this.aiActor.CorpseObject = (GameObject) null;
        this.aiActor.shadowDeathType = AIActor.ShadowDeathType.None;
        if ((bool) (UnityEngine.Object) this.aiActor.encounterTrackable)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.aiActor.encounterTrackable);
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnImagePreDeath);
        this.m_lastImageHealth = this.healthHaver.GetMaxHealth();
        this.aiAnimator.OtherAnimations[3].anim.Prefix = "poof";
        this.RegenerateCache();
        SeekTargetBehavior seekTargetBehavior = new SeekTargetBehavior();
        seekTargetBehavior.StopWhenInRange = false;
        this.behaviorSpeculator.InstantFirstTick = true;
        this.behaviorSpeculator.PostAwakenDelay = 0.0f;
        this.behaviorSpeculator.MovementBehaviors[0] = (MovementBehaviorBase) seekTargetBehavior;
        AttackBehaviorGroup attackBehaviorGroup = this.behaviorSpeculator.AttackBehaviorGroup;
        if (attackBehaviorGroup != null)
        {
          attackBehaviorGroup.AttackBehaviors[0].Probability = 0.0f;
          attackBehaviorGroup.AttackBehaviors[1].Probability = 1f;
        }
        foreach (Behaviour componentsInChild in this.GetComponentsInChildren<BulletLimbController>())
          componentsInChild.enabled = true;
        this.Fade = 0.0f;
        this.aiActor.SetOutlines(true);
        this.UpdateOutlineMaterial(this.sprite);
        this.healthHaver.OnDeath += new Action<Vector2>(this.OnImageDeath);
        this.m_initialized = true;
      }

      public void SetHost(AIActor host)
      {
        this.m_host = host;
        if (!(bool) (UnityEngine.Object) this.m_host)
          return;
        this.aiAnimator.CopyStateFrom(this.m_host.aiAnimator);
        this.m_lastHostHealth = host.healthHaver.GetMaxHealth();
        this.m_host.healthHaver.OnPreDeath += new Action<Vector2>(this.OnHostPreDeath);
        this.m_host.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnHostDamaged);
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnImageDamaged);
        host.SetOutlines(true);
        this.UpdateOutlineMaterial(host.sprite);
        this.OnFadeChange(this.m_host, 1f - this.Fade, true);
      }

      public void ClearHost()
      {
        if ((UnityEngine.Object) this.m_host == (UnityEngine.Object) null)
          return;
        this.m_host.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnHostPreDeath);
        this.m_host.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnHostDamaged);
        this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnImageDamaged);
        this.OnFadeChange(this.m_host, 0.0f, true);
        this.m_host = (AIActor) null;
      }

      private void OnHostPreDeath(Vector2 deathDir)
      {
        this.ClearHost();
        this.healthHaver.ApplyDamage(100000f, Vector2.zero, "Mirror Host Death", damageCategory: DamageCategory.Unstoppable);
      }

      private void OnImagePreDeath(Vector2 deathDir)
      {
        this.OnFadeChange(this.aiActor, 0.0f, false);
        this.StartCoroutine(this.DeathFade());
      }

      private void OnHostDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damagetypes,
        DamageCategory damagecategory,
        Vector2 damagedirection)
      {
        this.OnEitherDamaged(this.m_lastHostHealth - resultValue, maxValue);
        this.m_lastHostHealth = resultValue;
      }

      private void OnImageDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damagetypes,
        DamageCategory damagecategory,
        Vector2 damagedirection)
      {
        this.OnEitherDamaged(this.m_lastImageHealth - resultValue, maxValue);
        this.m_lastImageHealth = resultValue;
      }

      private void OnImageDeath(Vector2 vector2) => this.aiAnimator.PlayVfx("death_poof");

      private void OnFadeChange(AIActor aiActor, float fade, bool isHost)
      {
        if (!(bool) (UnityEngine.Object) aiActor)
          return;
        aiActor.renderer.material.SetFloat("_DisplacerFade", fade * 1.5f);
        aiActor.sprite.usesOverrideMaterial = (double) fade > 0.0;
        if (isHost)
        {
          if ((double) fade <= 0.0)
          {
            aiActor.SetOutlines(true);
            this.UpdateOutlineMaterial(aiActor.sprite);
          }
          else
          {
            bool flag = false;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
              if ((bool) (UnityEngine.Object) allPlayer && allPlayer.CanDetectHiddenEnemies)
                flag = true;
            }
            if (!flag)
              aiActor.SetOutlines(false);
          }
        }
        tk2dSprite component = aiActor.ShadowObject.GetComponent<tk2dSprite>();
        component.color = component.color.WithAlpha(1f - fade);
      }

      private void OnEitherDamaged(float damage, float maxHealth)
      {
        this.Fade = Mathf.Clamp01(this.Fade + damage / maxHealth / this.DamagePercentForMaxFade);
        this.m_unfadeDelayTimer = this.UnfadeDelayTime;
      }

      [DebuggerHidden]
      private IEnumerator DeathFade()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DisplacedImageController.<DeathFade>c__Iterator0()
        {
          $this = this
        };
      }

      private void UpdateOutlineMaterial(tk2dBaseSprite sprite)
      {
        Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(sprite);
        outlineMaterial.SetColor("_OverrideColor", new Color(0.0f, 11f, 33f));
        outlineMaterial.EnableKeyword("EXCLUDE_INTERIOR");
        outlineMaterial.DisableKeyword("INCLUDE_INTERIOR");
      }
    }

}
