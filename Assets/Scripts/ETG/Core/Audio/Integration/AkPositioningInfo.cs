// Decompiled with JetBrains decompiler
// Type: AkPositioningInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkPositioningInfo : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkPositioningInfo(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkPositioningInfo()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkPositioningInfo(), true)
    {
    }

    internal static IntPtr getCPtr(AkPositioningInfo obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkPositioningInfo() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkPositioningInfo(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public float fCenterPct
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fCenterPct_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fCenterPct_get(this.swigCPtr);
    }

    public AkPannerType pannerType
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_pannerType_set(this.swigCPtr, (int) value);
      get
      {
        return (AkPannerType) AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_pannerType_get(this.swigCPtr);
      }
    }

    public AkPositionSourceType posSourceType
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_posSourceType_set(this.swigCPtr, (int) value);
      }
      get
      {
        return (AkPositionSourceType) AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_posSourceType_get(this.swigCPtr);
      }
    }

    public bool bUpdateEachFrame
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUpdateEachFrame_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUpdateEachFrame_get(this.swigCPtr);
    }

    public Ak3DSpatializationMode e3DSpatializationMode
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_e3DSpatializationMode_set(this.swigCPtr, (int) value);
      }
      get
      {
        return (Ak3DSpatializationMode) AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_e3DSpatializationMode_get(this.swigCPtr);
      }
    }

    public bool bUseAttenuation
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseAttenuation_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseAttenuation_get(this.swigCPtr);
    }

    public bool bUseConeAttenuation
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseConeAttenuation_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseConeAttenuation_get(this.swigCPtr);
    }

    public float fInnerAngle
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fInnerAngle_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fInnerAngle_get(this.swigCPtr);
    }

    public float fOuterAngle
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fOuterAngle_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fOuterAngle_get(this.swigCPtr);
    }

    public float fConeMaxAttenuation
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fConeMaxAttenuation_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fConeMaxAttenuation_get(this.swigCPtr);
    }

    public float LPFCone
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFCone_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFCone_get(this.swigCPtr);
    }

    public float HPFCone
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFCone_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFCone_get(this.swigCPtr);
    }

    public float fMaxDistance
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fMaxDistance_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fMaxDistance_get(this.swigCPtr);
    }

    public float fVolDryAtMaxDist
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolDryAtMaxDist_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolDryAtMaxDist_get(this.swigCPtr);
    }

    public float fVolAuxGameDefAtMaxDist
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxGameDefAtMaxDist_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxGameDefAtMaxDist_get(this.swigCPtr);
    }

    public float fVolAuxUserDefAtMaxDist
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxUserDefAtMaxDist_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxUserDefAtMaxDist_get(this.swigCPtr);
    }

    public float LPFValueAtMaxDist
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFValueAtMaxDist_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFValueAtMaxDist_get(this.swigCPtr);
    }

    public float HPFValueAtMaxDist
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFValueAtMaxDist_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFValueAtMaxDist_get(this.swigCPtr);
    }
  }

