using System;

#nullable disable

public class AkThreadProperties : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkThreadProperties(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkThreadProperties()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkThreadProperties(), true)
    {
    }

    internal static IntPtr getCPtr(AkThreadProperties obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkThreadProperties() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkThreadProperties(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public int nPriority
    {
      set => AkSoundEnginePINVOKE.CSharp_AkThreadProperties_nPriority_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkThreadProperties_nPriority_get(this.swigCPtr);
    }

    public uint dwAffinityMask
    {
      set => AkSoundEnginePINVOKE.CSharp_AkThreadProperties_dwAffinityMask_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkThreadProperties_dwAffinityMask_get(this.swigCPtr);
    }

    public uint uStackSize
    {
      set => AkSoundEnginePINVOKE.CSharp_AkThreadProperties_uStackSize_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkThreadProperties_uStackSize_get(this.swigCPtr);
    }
  }

