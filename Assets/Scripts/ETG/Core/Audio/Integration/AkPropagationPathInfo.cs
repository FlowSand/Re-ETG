using System;

#nullable disable

public class AkPropagationPathInfo : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;
    public const uint kMaxNodes = 8;

    internal AkPropagationPathInfo(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkPropagationPathInfo()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkPropagationPathInfo(), true)
    {
    }

    internal static IntPtr getCPtr(AkPropagationPathInfo obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkPropagationPathInfo() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkPropagationPathInfo(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public AkVector nodePoint
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_nodePoint_set(this.swigCPtr, AkVector.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_nodePoint_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : (AkVector) null;
      }
    }

    public uint numNodes
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_numNodes_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_numNodes_get(this.swigCPtr);
    }

    public float length
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_length_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_length_get(this.swigCPtr);
    }

    public float gain
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_gain_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_gain_get(this.swigCPtr);
    }

    public float dryDiffractionAngle
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_dryDiffractionAngle_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_dryDiffractionAngle_get(this.swigCPtr);
    }

    public float wetDiffractionAngle
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_wetDiffractionAngle_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPropagationPathInfo_wetDiffractionAngle_get(this.swigCPtr);
    }
  }

