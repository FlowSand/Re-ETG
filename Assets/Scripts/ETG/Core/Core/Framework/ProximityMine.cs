// Decompiled with JetBrains decompiler
// Type: ProximityMine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ProximityMine : BraveBehaviour
    {
      public ExplosionData explosionData;
      public ProximityMine.ExplosiveStyle explosionStyle;
      [ShowInInspectorIf("explosionStyle", 0, false)]
      public float detectRadius = 2.5f;
      public float explosionDelay = 0.3f;
      public bool usesCustomExplosionDelay;
      [ShowInInspectorIf("usesCustomExplosionDelay", false)]
      public float customExplosionDelay = 0.1f;
      [CheckAnimation(null)]
      public string deployAnimName;
      [CheckAnimation(null)]
      public string idleAnimName;
      [CheckAnimation(null)]
      public string explodeAnimName;
      [Header("Homing")]
      public bool MovesTowardEnemies;
      public bool HomingTriggeredOnSynergy;
      [LongNumericEnum]
      public CustomSynergyType TriggerSynergy;
      public float HomingRadius = 5f;
      public float HomingSpeed = 3f;
      public float HomingDelay = 5f;
      protected bool m_triggered;
      protected bool m_disarmed;

      private void TransitionToIdle(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
      {
        if (this.idleAnimName != null && !animator.IsPlaying(this.explodeAnimName))
          animator.Play(this.idleAnimName);
        animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToIdle);
      }

      private void Update()
      {
        if (!this.MovesTowardEnemies && this.HomingTriggeredOnSynergy && GameManager.Instance.PrimaryPlayer.HasActiveBonusSynergy(this.TriggerSynergy))
          this.MovesTowardEnemies = true;
        if (!this.MovesTowardEnemies)
          return;
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        float nearestDistance = float.MaxValue;
        AIActor nearestEnemy = absoluteRoom.GetNearestEnemy(this.sprite.WorldCenter, out nearestDistance);
        if (!(bool) (UnityEngine.Object) nearestEnemy || (double) nearestDistance >= (double) this.HomingRadius)
          return;
        Vector2 normalized = (nearestEnemy.CenterPosition - this.sprite.WorldCenter).normalized;
        if ((bool) (UnityEngine.Object) this.debris)
          this.debris.ApplyFrameVelocity(normalized * this.HomingSpeed);
        else
          this.transform.position = this.transform.position + normalized.ToVector3ZisY() * this.HomingSpeed * BraveTime.DeltaTime;
      }

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ProximityMine.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();

      public enum ExplosiveStyle
      {
        PROXIMITY,
        TIMED,
      }
    }

}
