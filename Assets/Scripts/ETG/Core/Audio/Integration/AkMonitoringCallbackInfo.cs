// Decompiled with JetBrains decompiler
// Type: AkMonitoringCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkMonitoringCallbackInfo : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkMonitoringCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkMonitoringCallbackInfo()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMonitoringCallbackInfo(), true)
      {
      }

      internal static IntPtr getCPtr(AkMonitoringCallbackInfo obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkMonitoringCallbackInfo() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMonitoringCallbackInfo(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public AkMonitorErrorCode errorCode
      {
        get
        {
          return (AkMonitorErrorCode) AkSoundEnginePINVOKE.CSharp_AkMonitoringCallbackInfo_errorCode_get(this.swigCPtr);
        }
      }

      public AkMonitorErrorLevel errorLevel
      {
        get
        {
          return (AkMonitorErrorLevel) AkSoundEnginePINVOKE.CSharp_AkMonitoringCallbackInfo_errorLevel_get(this.swigCPtr);
        }
      }

      public uint playingID
      {
        get => AkSoundEnginePINVOKE.CSharp_AkMonitoringCallbackInfo_playingID_get(this.swigCPtr);
      }

      public ulong gameObjID
      {
        get => AkSoundEnginePINVOKE.CSharp_AkMonitoringCallbackInfo_gameObjID_get(this.swigCPtr);
      }

      public string message
      {
        get
        {
          return AkSoundEngine.StringFromIntPtrOSString(AkSoundEnginePINVOKE.CSharp_AkMonitoringCallbackInfo_message_get(this.swigCPtr));
        }
      }
    }

}
