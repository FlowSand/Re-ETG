// Decompiled with JetBrains decompiler
// Type: AkObjectInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkObjectInfo : IDisposable
{
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkObjectInfo(IntPtr cPtr, bool cMemoryOwn)
  {
    this.swigCMemOwn = cMemoryOwn;
    this.swigCPtr = cPtr;
  }

  public AkObjectInfo()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkObjectInfo(), true)
  {
  }

  internal static IntPtr getCPtr(AkObjectInfo obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

  internal virtual void setCPtr(IntPtr cPtr)
  {
    this.Dispose();
    this.swigCPtr = cPtr;
  }

  ~AkObjectInfo() => this.Dispose();

  public virtual void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkObjectInfo(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
    }
  }

  public uint objID
  {
    set => AkSoundEnginePINVOKE.CSharp_AkObjectInfo_objID_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkObjectInfo_objID_get(this.swigCPtr);
  }

  public uint parentID
  {
    set => AkSoundEnginePINVOKE.CSharp_AkObjectInfo_parentID_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkObjectInfo_parentID_get(this.swigCPtr);
  }

  public int iDepth
  {
    set => AkSoundEnginePINVOKE.CSharp_AkObjectInfo_iDepth_set(this.swigCPtr, value);
    get => AkSoundEnginePINVOKE.CSharp_AkObjectInfo_iDepth_get(this.swigCPtr);
  }
}
