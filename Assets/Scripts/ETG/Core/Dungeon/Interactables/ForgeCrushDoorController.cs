// Decompiled with JetBrains decompiler
// Type: ForgeCrushDoorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ForgeCrushDoorController : DungeonPlaceableBehaviour
    {
      public float DamageToEnemies = 30f;
      public float KnockbackForcePlayers = 50f;
      public float KnockbackForceEnemies = 50f;
      public bool DoScreenShake;
      public ScreenShakeSettings ScreenShake;
      public string CloseAnimName;
      public string OpenAnimName;
      public tk2dSpriteAnimator SubsidiaryAnimator;
      public string SubsidiaryCloseAnimName;
      public string SubsidiaryOpenAnimName;
      public tk2dSpriteAnimator vfxAnimator;
      public float DelayTime = 0.25f;
      public float TimeClosed = 1f;
      public float CooldownTime = 3f;
      private bool m_isCrushing;

      private void Start()
      {
        if ((UnityEngine.Object) this.specRigidbody == (UnityEngine.Object) null)
          this.specRigidbody = this.GetComponentInChildren<SpeculativeRigidbody>();
        if ((UnityEngine.Object) this.spriteAnimator == (UnityEngine.Object) null)
          this.spriteAnimator = this.GetComponentInChildren<tk2dSpriteAnimator>();
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTrigger);
        this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
        this.spriteAnimator.Sprite.UpdateZDepth();
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void HandleTrigger(
        SpeculativeRigidbody specRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (this.m_isCrushing)
          return;
        this.m_isCrushing = true;
        this.StartCoroutine(this.HandleCrush());
      }

      [DebuggerHidden]
      private IEnumerator HandleCrush()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ForgeCrushDoorController__HandleCrushc__Iterator0()
        {
          _this = this
        };
      }

      private void HandleAnimationEvent(
        tk2dSpriteAnimator sourceAnimator,
        tk2dSpriteAnimationClip sourceClip,
        int sourceFrame)
      {
        if (!(sourceClip.frames[sourceFrame].eventInfo == "impact"))
          return;
        if (this.DoScreenShake)
          GameManager.Instance.MainCameraController.DoScreenShake(this.ScreenShake, new Vector2?(this.specRigidbody.UnitCenter));
        this.specRigidbody.PixelColliders[1].Enabled = true;
        this.specRigidbody.Reinitialize();
        Exploder.DoRadialMinorBreakableBreak((Vector3) this.spriteAnimator.Sprite.WorldCenter, 1f);
        List<SpeculativeRigidbody> overlappingRigidbodies = PhysicsEngine.Instance.GetOverlappingRigidbodies(this.specRigidbody);
        for (int index = 0; index < overlappingRigidbodies.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].gameActor)
          {
            Vector2 direction = overlappingRigidbodies[index].UnitCenter - this.specRigidbody.UnitCenter;
            if (overlappingRigidbodies[index].gameActor is PlayerController)
            {
              if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].healthHaver)
                overlappingRigidbodies[index].healthHaver.ApplyDamage(0.5f, direction, StringTableManager.GetEnemiesString("#TRAP"));
              if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].knockbackDoer)
                overlappingRigidbodies[index].knockbackDoer.ApplyKnockback(direction, this.KnockbackForcePlayers);
            }
            else
            {
              if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].healthHaver)
                overlappingRigidbodies[index].healthHaver.ApplyDamage(this.DamageToEnemies, direction, StringTableManager.GetEnemiesString("#TRAP"));
              if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].knockbackDoer)
                overlappingRigidbodies[index].knockbackDoer.ApplyKnockback(direction, this.KnockbackForceEnemies);
            }
          }
        }
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
      }
    }

}
