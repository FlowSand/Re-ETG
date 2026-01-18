// Decompiled with JetBrains decompiler
// Type: _ArrayPoolSpatialAudio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class _ArrayPoolSpatialAudio : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal _ArrayPoolSpatialAudio(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public _ArrayPoolSpatialAudio()
      : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolSpatialAudio(), true)
    {
    }

    internal static IntPtr getCPtr(_ArrayPoolSpatialAudio obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~_ArrayPoolSpatialAudio() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolSpatialAudio(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public static int Get() => AkSoundEnginePINVOKE.CSharp__ArrayPoolSpatialAudio_Get();
  }

