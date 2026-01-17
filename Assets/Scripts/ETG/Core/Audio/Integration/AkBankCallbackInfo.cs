// Decompiled with JetBrains decompiler
// Type: AkBankCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkBankCallbackInfo : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkBankCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkBankCallbackInfo()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkBankCallbackInfo(), true)
      {
      }

      internal static IntPtr getCPtr(AkBankCallbackInfo obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkBankCallbackInfo() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkBankCallbackInfo(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public uint bankID => AkSoundEnginePINVOKE.CSharp_AkBankCallbackInfo_bankID_get(this.swigCPtr);

      public IntPtr inMemoryBankPtr
      {
        get => AkSoundEnginePINVOKE.CSharp_AkBankCallbackInfo_inMemoryBankPtr_get(this.swigCPtr);
      }

      public AKRESULT loadResult
      {
        get => (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkBankCallbackInfo_loadResult_get(this.swigCPtr);
      }

      public int memPoolId
      {
        get => AkSoundEnginePINVOKE.CSharp_AkBankCallbackInfo_memPoolId_get(this.swigCPtr);
      }
    }

}
