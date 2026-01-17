// Decompiled with JetBrains decompiler
// Type: AkObstructionOcclusionValues
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
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

}
