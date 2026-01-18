using System;

#nullable disable

public class AkSoundPropagationPathParams : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkSoundPropagationPathParams(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkSoundPropagationPathParams()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkSoundPropagationPathParams(), true)
    {
    }

    internal static IntPtr getCPtr(AkSoundPropagationPathParams obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkSoundPropagationPathParams() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkSoundPropagationPathParams(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public AkVector listenerPos
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkSoundPropagationPathParams_listenerPos_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPropagationPathParams_listenerPos_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public AkVector emitterPos
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkSoundPropagationPathParams_emitterPos_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPropagationPathParams_emitterPos_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public uint numValidPaths
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkSoundPropagationPathParams_numValidPaths_set(this.swigCPtr, value);
      }
      get
      {
        return AkSoundEnginePINVOKE.CSharp_AkSoundPropagationPathParams_numValidPaths_get(this.swigCPtr);
      }
    }
  }

