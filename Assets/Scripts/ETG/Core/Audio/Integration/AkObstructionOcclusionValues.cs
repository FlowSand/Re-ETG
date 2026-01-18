using System;

#nullable disable

public class AkObstructionOcclusionValues : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkObstructionOcclusionValues(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkObstructionOcclusionValues()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkObstructionOcclusionValues(), true)
    {
    }

    internal static IntPtr getCPtr(AkObstructionOcclusionValues obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkObstructionOcclusionValues() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkObstructionOcclusionValues(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public float occlusion
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkObstructionOcclusionValues_occlusion_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkObstructionOcclusionValues_occlusion_get(this.swigCPtr);
    }

    public float obstruction
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkObstructionOcclusionValues_obstruction_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkObstructionOcclusionValues_obstruction_get(this.swigCPtr);
    }
  }

