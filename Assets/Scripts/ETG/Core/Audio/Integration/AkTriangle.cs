// Decompiled with JetBrains decompiler
// Type: AkTriangle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkTriangle : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkTriangle(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkTriangle()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkTriangle(), true)
    {
    }

    internal static IntPtr getCPtr(AkTriangle obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkTriangle() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkTriangle(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public AkVector point0
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkTriangle_point0_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkTriangle_point0_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public AkVector point1
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkTriangle_point1_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkTriangle_point1_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public AkVector point2
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkTriangle_point2_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkTriangle_point2_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public uint textureID
    {
      set => AkSoundEnginePINVOKE.CSharp_AkTriangle_textureID_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkTriangle_textureID_get(this.swigCPtr);
    }

    public uint reflectorChannelMask
    {
      set => AkSoundEnginePINVOKE.CSharp_AkTriangle_reflectorChannelMask_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkTriangle_reflectorChannelMask_get(this.swigCPtr);
    }

    public string strName
    {
      set => AkSoundEnginePINVOKE.CSharp_AkTriangle_strName_set(this.swigCPtr, value);
      get
      {
        return AkSoundEngine.StringFromIntPtrString(AkSoundEnginePINVOKE.CSharp_AkTriangle_strName_get(this.swigCPtr));
      }
    }
  }

