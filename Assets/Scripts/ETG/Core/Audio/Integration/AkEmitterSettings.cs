// Decompiled with JetBrains decompiler
// Type: AkEmitterSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public class AkEmitterSettings : IDisposable
    {
      private IntPtr swigCPtr;
      protected bool swigCMemOwn;

      internal AkEmitterSettings(IntPtr cPtr, bool cMemoryOwn)
      {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
      }

      public AkEmitterSettings()
        : this(AkSoundEnginePINVOKE.CSharp_new_AkEmitterSettings(), true)
      {
      }

      internal static IntPtr getCPtr(AkEmitterSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

      internal virtual void setCPtr(IntPtr cPtr)
      {
        this.Dispose();
        this.swigCPtr = cPtr;
      }

      ~AkEmitterSettings() => this.Dispose();

      public virtual void Dispose()
      {
        lock ((object) this)
        {
          if (this.swigCPtr != IntPtr.Zero)
          {
            if (this.swigCMemOwn)
            {
              this.swigCMemOwn = false;
              AkSoundEnginePINVOKE.CSharp_delete_AkEmitterSettings(this.swigCPtr);
            }
            this.swigCPtr = IntPtr.Zero;
          }
          GC.SuppressFinalize((object) this);
        }
      }

      public uint reflectAuxBusID
      {
        set => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectAuxBusID_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectAuxBusID_get(this.swigCPtr);
      }

      public float reflectionMaxPathLength
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectionMaxPathLength_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectionMaxPathLength_get(this.swigCPtr);
      }

      public float reflectionsAuxBusGain
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectionsAuxBusGain_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectionsAuxBusGain_get(this.swigCPtr);
      }

      public uint reflectionsOrder
      {
        set => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectionsOrder_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectionsOrder_get(this.swigCPtr);
      }

      public uint reflectorFilterMask
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectorFilterMask_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_reflectorFilterMask_get(this.swigCPtr);
      }

      public float roomReverbAuxBusGain
      {
        set
        {
          AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_roomReverbAuxBusGain_set(this.swigCPtr, value);
        }
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_roomReverbAuxBusGain_get(this.swigCPtr);
      }

      public byte useImageSources
      {
        set => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_useImageSources_set(this.swigCPtr, value);
        get => AkSoundEnginePINVOKE.CSharp_AkEmitterSettings_useImageSources_get(this.swigCPtr);
      }
    }

}
