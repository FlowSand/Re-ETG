// Decompiled with JetBrains decompiler
// Type: AkAuxSendValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkAuxSendValue : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkAuxSendValue(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      internal static IntPtr getCPtr(AkAuxSendValue obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkAuxSendValue() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkAuxSendValue(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public ulong listenerID
      {
        set => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_listenerID_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_listenerID_get(this.swigCPtr);
      }

      public uint auxBusID
      {
        set => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_auxBusID_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_auxBusID_get(this.swigCPtr);
      }

      public float fControlValue
      {
        set => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_fControlValue_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_fControlValue_get(this.swigCPtr);
      }

      public void Set(GameObject listener, uint id, float value)
      {
        ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(listener);
        AkSoundEngine.PreGameObjectAPICall(listener, akGameObjectId);
        AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_Set(this.swigCPtr, akGameObjectId, id, value);
      }

      public bool IsSame(GameObject listener, uint id)
      {
        ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(listener);
        AkSoundEngine.PreGameObjectAPICall(listener, akGameObjectId);
        return AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_IsSame(this.swigCPtr, akGameObjectId, id);
      }

      public static int GetSizeOf() => AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_GetSizeOf();

      public AKRESULT SetGameObjectAuxSendValues(GameObject in_gameObjectID, uint in_uNumSendValues)
      {
        ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
        AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, akGameObjectId);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_SetGameObjectAuxSendValues(this.swigCPtr, akGameObjectId, in_uNumSendValues);
      }

      public AKRESULT GetGameObjectAuxSendValues(
        GameObject in_gameObjectID,
        ref uint io_ruNumSendValues)
      {
        ulong akGameObjectId = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
        AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, akGameObjectId);
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_GetGameObjectAuxSendValues(this.swigCPtr, akGameObjectId, ref io_ruNumSendValues);
      }
    }

}
