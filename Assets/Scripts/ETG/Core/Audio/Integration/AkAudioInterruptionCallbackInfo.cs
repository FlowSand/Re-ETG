// Decompiled with JetBrains decompiler
// Type: AkAudioInterruptionCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkAudioInterruptionCallbackInfo : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkAudioInterruptionCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkAudioInterruptionCallbackInfo()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkAudioInterruptionCallbackInfo(), true)
      {
      }

      internal static IntPtr getCPtr(AkAudioInterruptionCallbackInfo obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkAudioInterruptionCallbackInfo() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkAudioInterruptionCallbackInfo(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public bool bEnterInterruption
      {
        get
        {
          return AkSoundEnginePINVOKE.CSharp_AkAudioInterruptionCallbackInfo_bEnterInterruption_get(this.swigCPtr);
        }
      }
    }

}
