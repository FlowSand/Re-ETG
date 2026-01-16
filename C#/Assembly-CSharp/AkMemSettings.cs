// Decompiled with JetBrains decompiler
// Type: AkMemSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkMemSettings : IDisposable
{
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkMemSettings(IntPtr cPtr, bool cMemoryOwn)
  {
    this.swigCMemOwn = cMemoryOwn;
    this.swigCPtr = cPtr;
  }

  public AkMemSettings()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkMemSettings(), true)
  {
  }

  internal static IntPtr getCPtr(AkMemSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

  internal virtual void setCPtr(IntPtr cPtr)
  {
    this.Dispose();
    this.swigCPtr = cPtr;
  }

  ~AkMemSettings() => this.Dispose();

  public virtual void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkMemSettings(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
    }
  }

  public uint uMaxNumPools
  {
    set => AkSoundEnginePINVOKE.CSharp_AkMemSettings_uMaxNumPools_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkMemSettings_uMaxNumPools_get(this.swigCPtr);
  }

  public uint uDebugFlags
  {
    set => AkSoundEnginePINVOKE.CSharp_AkMemSettings_uDebugFlags_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkMemSettings_uDebugFlags_get(this.swigCPtr);
  }
}
