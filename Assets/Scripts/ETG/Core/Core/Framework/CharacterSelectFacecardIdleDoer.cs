// Decompiled with JetBrains decompiler
// Type: CharacterSelectFacecardIdleDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class CharacterSelectFacecardIdleDoer : BraveBehaviour
    {
      public string appearAnimation = "_appear";
      public string coreIdleAnimation;
      public float idleMin = 4f;
      public float idleMax = 10f;
      public bool usesMultipleIdleAnimations;
      public string[] multipleIdleAnimations;
      public Texture2D EeveeTex;
      protected int lastPhase = -1;

      private void OnEnable()
      {
        if ((bool) (Object) this.EeveeTex)
        {
          this.sprite.usesOverrideMaterial = true;
          this.sprite.renderer.material.shader = Shader.Find("Brave/Internal/GlitchEevee");
          this.sprite.renderer.sharedMaterial.SetTexture("_EeveeTex", (Texture) this.EeveeTex);
        }
        this.spriteAnimator.Play(this.appearAnimation);
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
        return (IEnumerator) new CharacterSelectFacecardIdleDoer.\u003CHandleCoreIdle\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
