// Decompiled with JetBrains decompiler
// Type: AkOutputSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AkOutputSettings : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkOutputSettings(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkOutputSettings()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings__SWIG_0(), true)
    {
    }

    public AkOutputSettings(
      string in_szDeviceShareSet,
      uint in_idDevice,
      AkChannelConfig in_channelConfig,
      AkPanningRule in_ePanning)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings__SWIG_1(in_szDeviceShareSet, in_idDevice, AkChannelConfig.getCPtr(in_channelConfig), (int) in_ePanning), true)
    {
    }

    public AkOutputSettings(
      string in_szDeviceShareSet,
      uint in_idDevice,
      AkChannelConfig in_channelConfig)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings__SWIG_2(in_szDeviceShareSet, in_idDevice, AkChannelConfig.getCPtr(in_channelConfig)), true)
    {
    }

    public AkOutputSettings(string in_szDeviceShareSet, uint in_idDevice)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings__SWIG_3(in_szDeviceShareSet, in_idDevice), true)
    {
    }

    public AkOutputSettings(string in_szDeviceShareSet)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings__SWIG_4(in_szDeviceShareSet), true)
    {
    }

    internal static IntPtr getCPtr(AkOutputSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkOutputSettings() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkOutputSettings(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public uint audioDeviceShareset
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkOutputSettings_audioDeviceShareset_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkOutputSettings_audioDeviceShareset_get(this.swigCPtr);
    }

    public uint idDevice
    {
      set => AkSoundEnginePINVOKE.CSharp_AkOutputSettings_idDevice_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkOutputSettings_idDevice_get(this.swigCPtr);
    }

    public AkPanningRule ePanningRule
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkOutputSettings_ePanningRule_set(this.swigCPtr, (int) value);
      }
      get
      {
        return (AkPanningRule) AkSoundEnginePINVOKE.CSharp_AkOutputSettings_ePanningRule_get(this.swigCPtr);
      }
    }

    public AkChannelConfig channelConfig
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkOutputSettings_channelConfig_set(this.swigCPtr, AkChannelConfig.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkOutputSettings_channelConfig_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkChannelConfig(cPtr, false) : (AkChannelConfig) null;
      }
    }
  }

