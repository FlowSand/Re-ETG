// Decompiled with JetBrains decompiler
// Type: AkMarkerCallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkMarkerCallbackInfo : AkEventCallbackInfo
    {
      private IntPtr swigCPtr;

      internal AkMarkerCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
        : base(AkSoundEnginePINVOKE.CSharp_AkMarkerCallbackInfo_SWIGUpcast(cPtr), cMemoryOwn)
      {
        this.swigCPtr = cPtr;
      }

      public AkMarkerCallbackInfo()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkMarkerCallbackInfo(), true)
      {
      }

      internal static IntPtr getCPtr(AkMarkerCallbackInfo obj)
      {
        return obj == null ? IntPtr.Zero : obj.swigCPtr;
      }

      internal override void setCPtr(IntPtr cPtr)
      {
        base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkMarkerCallbackInfo_SWIGUpcast(cPtr));
        this.swigCPtr = cPtr;
      }

      ~AkMarkerCallbackInfo() => this.Dispose();

      public override void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkMarkerCallbackInfo(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
          base.Dispose();
        }
      }

      public uint uIdentifier
      {
        get => AkSoundEnginePINVOKE.CSharp_AkMarkerCallbackInfo_uIdentifier_get(this.swigCPtr);
      }

      public uint uPosition
      {
        get => AkSoundEnginePINVOKE.CSharp_AkMarkerCallbackInfo_uPosition_get(this.swigCPtr);
      }

      public string strLabel
      {
        get
        {
          return AkSoundEngine.StringFromIntPtrString(AkSoundEnginePINVOKE.CSharp_AkMarkerCallbackInfo_strLabel_get(this.swigCPtr));
        }
      }
    }

}
