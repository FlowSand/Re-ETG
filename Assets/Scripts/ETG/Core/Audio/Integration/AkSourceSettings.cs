// Decompiled with JetBrains decompiler
// Type: AkSourceSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkSourceSettings : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkSourceSettings(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkSourceSettings()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkSourceSettings(), true)
    {
    }

    internal static IntPtr getCPtr(AkSourceSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkSourceSettings() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkSourceSettings(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public uint sourceID
    {
      set => AkSoundEnginePINVOKE.CSharp_AkSourceSettings_sourceID_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkSourceSettings_sourceID_get(this.swigCPtr);
    }

    public IntPtr pMediaMemory
    {
      set => AkSoundEnginePINVOKE.CSharp_AkSourceSettings_pMediaMemory_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkSourceSettings_pMediaMemory_get(this.swigCPtr);
    }

    public uint uMediaSize
    {
      set => AkSoundEnginePINVOKE.CSharp_AkSourceSettings_uMediaSize_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkSourceSettings_uMediaSize_get(this.swigCPtr);
    }
  }

