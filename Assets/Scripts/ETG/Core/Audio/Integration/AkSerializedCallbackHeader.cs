// Decompiled with JetBrains decompiler
// Type: AkSerializedCallbackHeader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkSerializedCallbackHeader : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkSerializedCallbackHeader(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkSerializedCallbackHeader()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkSerializedCallbackHeader(), true)
      {
      }

      internal static IntPtr getCPtr(AkSerializedCallbackHeader obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkSerializedCallbackHeader() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkSerializedCallbackHeader(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public IntPtr pPackage
      {
        get => AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_pPackage_get(this.swigCPtr);
      }

      public AkSerializedCallbackHeader pNext
      {
        get
        {
          IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_pNext_get(this.swigCPtr);
          return !(cPtr == IntPtr.Zero) ? new AkSerializedCallbackHeader(cPtr, false) : (AkSerializedCallbackHeader) null;
        }
      }

      public AkCallbackType eType
      {
        get
        {
          return (AkCallbackType) AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_eType_get(this.swigCPtr);
        }
      }

      public IntPtr GetData()
      {
        return AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_GetData(this.swigCPtr);
      }
    }

}
