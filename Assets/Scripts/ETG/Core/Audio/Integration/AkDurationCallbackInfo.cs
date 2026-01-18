// Decompiled with JetBrains decompiler
// Type: AkDurationCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkDurationCallbackInfo : AkEventCallbackInfo
  {
    private IntPtr swigCPtr;

    internal AkDurationCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
      : base(AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_SWIGUpcast(cPtr), cMemoryOwn)
    {
      this.swigCPtr = cPtr;
    }

    public AkDurationCallbackInfo()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkDurationCallbackInfo(), true)
    {
    }

    internal static IntPtr getCPtr(AkDurationCallbackInfo obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal override void setCPtr(IntPtr cPtr)
    {
      base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_SWIGUpcast(cPtr));
      this.swigCPtr = cPtr;
    }

    ~AkDurationCallbackInfo() => this.Dispose();

    public override void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkDurationCallbackInfo(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
        base.Dispose();
      }
    }

    public float fDuration
    {
      get => AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_fDuration_get(this.swigCPtr);
    }

    public float fEstimatedDuration
    {
      get => AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_fEstimatedDuration_get(this.swigCPtr);
    }

    public uint audioNodeID
    {
      get => AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_audioNodeID_get(this.swigCPtr);
    }

    public uint mediaID
    {
      get => AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_mediaID_get(this.swigCPtr);
    }

    public bool bStreaming
    {
      get => AkSoundEnginePINVOKE.CSharp_AkDurationCallbackInfo_bStreaming_get(this.swigCPtr);
    }
  }

