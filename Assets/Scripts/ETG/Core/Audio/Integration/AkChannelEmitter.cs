// Decompiled with JetBrains decompiler
// Type: AkChannelEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkChannelEmitter : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkChannelEmitter(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkChannelEmitter()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkChannelEmitter(), true)
      {
      }

      internal static IntPtr getCPtr(AkChannelEmitter obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkChannelEmitter() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkChannelEmitter(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public AkTransform position
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkChannelEmitter_position_set(this.swigCPtr, AkTransform.getCPtr(value));
        }
        get
        {
          IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkChannelEmitter_position_get(this.swigCPtr);
          return !(cPtr == IntPtr.Zero) ? new AkTransform(cPtr, false) : (AkTransform) null;
        }
      }

      public uint uInputChannels
      {
        set => AkSoundEnginePINVOKE.CSharp_AkChannelEmitter_uInputChannels_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkChannelEmitter_uInputChannels_get(this.swigCPtr);
      }
    }

}
