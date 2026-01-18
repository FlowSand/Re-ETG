// Decompiled with JetBrains decompiler
// Type: InstantProjectileEffectModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;

#nullable disable

public class InstantProjectileEffectModifier : BraveBehaviour
  {
    public bool DoesWhiteFlash;
    public float RoomDamageRadius = 10f;
    public VFXPool AdditionalVFX;
    public bool DoesAdditionalScreenShake;
    [ShowInInspectorIf("DoesAdditionalScreenShake", false)]
    public ScreenShakeSettings AdditionalScreenShake;
    public bool DoesRadialProjectileModule;
    [ShowInInspectorIf("DoesRadialProjectileModule", false)]
    public RadialBurstInterface RadialModule;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InstantProjectileEffectModifier__Startc__Iterator0()
      {
        _this = this
      };
    }
  }

