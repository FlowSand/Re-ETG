// Decompiled with JetBrains decompiler
// Type: AkCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkCallbackInfo : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkCallbackInfo()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkCallbackInfo(), true)
    {
    }

    internal static IntPtr getCPtr(AkCallbackInfo obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkCallbackInfo() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkCallbackInfo(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public IntPtr pCookie => AkSoundEnginePINVOKE.CSharp_AkCallbackInfo_pCookie_get(this.swigCPtr);

    public ulong gameObjID => AkSoundEnginePINVOKE.CSharp_AkCallbackInfo_gameObjID_get(this.swigCPtr);
  }

