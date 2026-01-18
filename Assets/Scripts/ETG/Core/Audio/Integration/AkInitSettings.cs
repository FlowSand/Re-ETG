using System;

#nullable disable

public class AkInitSettings : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkInitSettings(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkInitSettings()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkInitSettings(), true)
    {
    }

    internal static IntPtr getCPtr(AkInitSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkInitSettings() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkInitSettings(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public int pfnAssertHook
    {
      set
      {
      }
      get => 0;
    }

    public uint uMaxNumPaths
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumPaths_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumPaths_get(this.swigCPtr);
    }

    public uint uDefaultPoolSize
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uDefaultPoolSize_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uDefaultPoolSize_get(this.swigCPtr);
    }

    public float fDefaultPoolRatioThreshold
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_fDefaultPoolRatioThreshold_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_fDefaultPoolRatioThreshold_get(this.swigCPtr);
    }

    public uint uCommandQueueSize
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCommandQueueSize_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCommandQueueSize_get(this.swigCPtr);
    }

    public int uPrepareEventMemoryPoolID
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_uPrepareEventMemoryPoolID_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uPrepareEventMemoryPoolID_get(this.swigCPtr);
    }

    public bool bEnableGameSyncPreparation
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_bEnableGameSyncPreparation_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_bEnableGameSyncPreparation_get(this.swigCPtr);
    }

    public uint uContinuousPlaybackLookAhead
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_uContinuousPlaybackLookAhead_set(this.swigCPtr, value);
      }
      get
      {
        return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uContinuousPlaybackLookAhead_get(this.swigCPtr);
      }
    }

    public uint uNumSamplesPerFrame
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uNumSamplesPerFrame_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uNumSamplesPerFrame_get(this.swigCPtr);
    }

    public uint uMonitorPoolSize
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorPoolSize_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorPoolSize_get(this.swigCPtr);
    }

    public uint uMonitorQueuePoolSize
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorQueuePoolSize_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorQueuePoolSize_get(this.swigCPtr);
    }

    public AkOutputSettings settingsMainOutput
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_settingsMainOutput_set(this.swigCPtr, AkOutputSettings.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkInitSettings_settingsMainOutput_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkOutputSettings(cPtr, false) : (AkOutputSettings) null;
      }
    }

    public uint uMaxHardwareTimeoutMs
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxHardwareTimeoutMs_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxHardwareTimeoutMs_get(this.swigCPtr);
    }

    public bool bUseSoundBankMgrThread
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseSoundBankMgrThread_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseSoundBankMgrThread_get(this.swigCPtr);
    }

    public bool bUseLEngineThread
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseLEngineThread_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseLEngineThread_get(this.swigCPtr);
    }

    public string szPluginDLLPath
    {
      set => AkSoundEnginePINVOKE.CSharp_AkInitSettings_szPluginDLLPath_set(this.swigCPtr, value);
      get
      {
        return AkSoundEngine.StringFromIntPtrOSString(AkSoundEnginePINVOKE.CSharp_AkInitSettings_szPluginDLLPath_get(this.swigCPtr));
      }
    }
  }

