using System;

#nullable disable

public class AkSoundPathInfo : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkSoundPathInfo(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkSoundPathInfo()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkSoundPathInfo(), true)
    {
    }

    internal static IntPtr getCPtr(AkSoundPathInfo obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkSoundPathInfo() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkSoundPathInfo(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public AkVector imageSource
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_imageSource_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_imageSource_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public uint numReflections
    {
      set => AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_numReflections_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_numReflections_get(this.swigCPtr);
    }

    public AkVector occlusionPoint
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_occlusionPoint_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_occlusionPoint_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public bool isOccluded
    {
      set => AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_isOccluded_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkSoundPathInfo_isOccluded_get(this.swigCPtr);
    }
  }

