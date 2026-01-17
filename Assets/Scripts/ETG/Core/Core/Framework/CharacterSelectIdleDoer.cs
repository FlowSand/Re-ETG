// Decompiled with JetBrains decompiler
// Type: CharacterSelectIdleDoer
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
    public class CharacterSelectIdleDoer : BraveBehaviour
    {
      public string coreIdleAnimation = "select_idle";
      public string onSelectedAnimation;
      public float idleMin = 4f;
      public float idleMax = 10f;
      public CharacterSelectIdlePhase[] phases;
      public bool IsEevee;
      public Texture2D EeveeTex;
      public tk2dSpriteAnimation[] AnimationLibraries;
      protected int lastPhase = -1;
      protected float m_lastEeveeSwitchTime;

      private void Update()
      {
        if (!this.IsEevee)
          return;
        this.sprite.usesOverrideMaterial = true;
        this.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
        this.sprite.renderer.sharedMaterial.SetTexture("_EeveeTex", (Texture) this.EeveeTex);
        this.m_lastEeveeSwitchTime += BraveTime.DeltaTime;
        if ((double) this.m_lastEeveeSwitchTime <= 2.5)
          return;
        this.m_lastEeveeSwitchTime -= 2.5f;
        this.spriteAnimator.Library = this.AnimationLibraries[UnityEngine.Random.Range(0, this.AnimationLibraries.Length)];
        this.spriteAnimator.Play(this.coreIdleAnimation);
      }

      private void OnEnable()
      {
        if (this.IsEevee)
        {
          this.sprite.usesOverrideMaterial = true;
          this.sprite.renderer.material.shader = Shader.Find("Brave/PlayerShaderEevee");
          this.sprite.renderer.sharedMaterial.SetTexture("_EeveeTex", (Texture) this.EeveeTex);
        }
        this.StartCoroutine(this.HandleCoreIdle());
      }

      private void OnDisable()
      {
        this.spriteAnimator.StopAndResetFrame();
        this.StopAllCoroutines();
      }

      [DebuggerHidden]
      private IEnumerator HandleCoreIdle()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CharacterSelectIdleDoer.\u003CHandleCoreIdle\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void DeactivateVFX(tk2dSpriteAnimator s, tk2dSpriteAnimationClip c)
      {
        s.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DeactivateVFX);
        s.gameObject.SetActive(false);
      }

      private void TriggerVFX(CharacterSelectIdlePhase phase)
      {
        phase.vfxSpriteAnimator.StopAndResetFrame();
        phase.vfxSpriteAnimator.gameObject.SetActive(true);
        phase.vfxSpriteAnimator.Play();
        phase.vfxSpriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DeactivateVFX);
      }

      private void TriggerEndVFX(CharacterSelectIdlePhase phase)
      {
        if (!((UnityEngine.Object) phase.endVFXSpriteAnimator != (UnityEngine.Object) null))
          return;
        phase.endVFXSpriteAnimator.StopAndResetFrame();
        phase.endVFXSpriteAnimator.Play();
      }

      [DebuggerHidden]
      private IEnumerator HandlePhase(CharacterSelectIdlePhase phase)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CharacterSelectIdleDoer.\u003CHandlePhase\u003Ec__Iterator1()
        {
          phase = phase,
          \u0024this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
