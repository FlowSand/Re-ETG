// Decompiled with JetBrains decompiler
// Type: AkStreamMgrSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkStreamMgrSettings : IDisposable
{
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkStreamMgrSettings(IntPtr cPtr, bool cMemoryOwn)
  {
    this.swigCMemOwn = cMemoryOwn;
    this.swigCPtr = cPtr;
  }

  public AkStreamMgrSettings()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkStreamMgrSettings(), true)
  {
  }

  internal static IntPtr getCPtr(AkStreamMgrSettings obj)
  {
    return obj == null ? IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(IntPtr cPtr)
  {
    this.Dispose();
    this.swigCPtr = cPtr;
  }

  ~AkStreamMgrSettings() => this.Dispose();

  public virtual void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkStreamMgrSettings(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
    }
  }

  public uint uMemorySize
  {
    set => AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_get(this.swigCPtr);
  }
}
