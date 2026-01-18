using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

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
      return (IEnumerator) new HologramFoyerController__Startc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator Core()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HologramFoyerController__Corec__Iterator1()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator CoreCycle(string targetAnimation)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HologramFoyerController__CoreCyclec__Iterator2()
      {
        targetAnimation = targetAnimation,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ToggleAdditionalLight(bool lightEnabled)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HologramFoyerController__ToggleAdditionalLightc__Iterator3()
      {
        lightEnabled = lightEnabled,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleArcLerp(bool invert)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HologramFoyerController__HandleArcLerpc__Iterator4()
      {
        invert = invert,
        _this = this
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

