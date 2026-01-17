// Decompiled with JetBrains decompiler
// Type: HologramFoyerController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HologramFoyerController : BraveBehaviour
    {
      public string[] animationCadence;
      public MeshRenderer ArcRenderer;
      private Material m_arcMaterial;
      public AdditionalBraveLight AttachedBraveLight;
      public tk2dSpriteAnimator TargetAnimator;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramFoyerController.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator Core()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramFoyerController.\u003CCore\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator CoreCycle(string targetAnimation)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramFoyerController.\u003CCoreCycle\u003Ec__Iterator2()
        {
          targetAnimation = targetAnimation,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ToggleAdditionalLight(bool lightEnabled)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramFoyerController.\u003CToggleAdditionalLight\u003Ec__Iterator3()
        {
          lightEnabled = lightEnabled,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleArcLerp(bool invert)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramFoyerController.\u003CHandleArcLerp\u003Ec__Iterator4()
        {
          invert = invert,
          \u0024this = this
        };
      }

      public void ChangeToAnimation(string animationName)
      {
        this.TargetAnimator.renderer.enabled = true;
        this.TargetAnimator.Play(animationName);
        this.TargetAnimator.Sprite.usesOverrideMaterial = true;
        this.TargetAnimator.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");
      }
    }

}
