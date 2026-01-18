// Decompiled with JetBrains decompiler
// Type: AkImageSourceParams
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkImageSourceParams : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkImageSourceParams(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkImageSourceParams()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkImageSourceParams__SWIG_0(), true)
    {
    }

    public AkImageSourceParams(
      AkVector in_sourcePosition,
      float in_fDistanceScalingFactor,
      float in_fLevel)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkImageSourceParams__SWIG_1(AkVector.getCPtr(in_sourcePosition), in_fDistanceScalingFactor, in_fLevel), true)
    {
    }

    internal static IntPtr getCPtr(AkImageSourceParams obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkImageSourceParams() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkImageSourceParams(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public AkVector sourcePosition
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkImageSourceParams_sourcePosition_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkImageSourceParams_sourcePosition_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public float fDistanceScalingFactor
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkImageSourceParams_fDistanceScalingFactor_set(this.swigCPtr, value);
      }
      get
      {
        return AkSoundEnginePINVOKE.CSharp_AkImageSourceParams_fDistanceScalingFactor_get(this.swigCPtr);
      }
    }

    public float fLevel
    {
      set => AkSoundEnginePINVOKE.CSharp_AkImageSourceParams_fLevel_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkImageSourceParams_fLevel_get(this.swigCPtr);
    }
  }

