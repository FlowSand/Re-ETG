// Decompiled with JetBrains decompiler
// Type: AkEventCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AkEventCallbackInfo : AkCallbackInfo
{
  private IntPtr swigCPtr;

  internal AkEventCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
    : base(AkSoundEnginePINVOKE.CSharp_AkEventCallbackInfo_SWIGUpcast(cPtr), cMemoryOwn)
  {
    this.swigCPtr = cPtr;
  }

  public AkEventCallbackInfo()
    : this(AkSoundEnginePINVOKE.CSharp_new_AkEventCallbackInfo(), true)
  {
  }

  internal static IntPtr getCPtr(AkEventCallbackInfo obj)
  {
    return obj == null ? IntPtr.Zero : obj.swigCPtr;
  }

  internal override void setCPtr(IntPtr cPtr)
  {
    base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkEventCallbackInfo_SWIGUpcast(cPtr));
    this.swigCPtr = cPtr;
  }

  ~AkEventCallbackInfo() => this.Dispose();

  public override void Dispose()
  {
    lock ((object) this)
    {
      if (this.swigCPtr != IntPtr.Zero)
      {
        if (this.swigCMemOwn)
        {
          this.swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkEventCallbackInfo(this.swigCPtr);
        }
        this.swigCPtr = IntPtr.Zero;
      }
      GC.SuppressFinalize((object) this);
      base.Dispose();
    }
  }

  public uint playingID
  {
    get => AkSoundEnginePINVOKE.CSharp_AkEventCallbackInfo_playingID_get(this.swigCPtr);
  }

  public uint eventID => AkSoundEnginePINVOKE.CSharp_AkEventCallbackInfo_eventID_get(this.swigCPtr);
}
