using System;

#nullable disable

public class AkRamp : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkRamp(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkRamp()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkRamp__SWIG_0(), true)
    {
    }

    public AkRamp(float in_fPrev, float in_fNext)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkRamp__SWIG_1(in_fPrev, in_fNext), true)
    {
    }

    internal static IntPtr getCPtr(AkRamp obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkRamp() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkRamp(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public float fPrev
    {
      set => AkSoundEnginePINVOKE.CSharp_AkRamp_fPrev_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkRamp_fPrev_get(this.swigCPtr);
    }

    public float fNext
    {
      set => AkSoundEnginePINVOKE.CSharp_AkRamp_fNext_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkRamp_fNext_get(this.swigCPtr);
    }
  }

