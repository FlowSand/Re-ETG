using System;

#nullable disable

public class AkPlatformInitSettings : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkPlatformInitSettings(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkPlatformInitSettings()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkPlatformInitSettings(), true)
    {
    }

    internal static IntPtr getCPtr(AkPlatformInitSettings obj)
    {
      return obj == null ? IntPtr.Zero : obj.swigCPtr;
    }

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkPlatformInitSettings() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkPlatformInitSettings(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public AkThreadProperties threadLEngine
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadLEngine_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadLEngine_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : (AkThreadProperties) null;
      }
    }

    public AkThreadProperties threadBankManager
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadBankManager_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadBankManager_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : (AkThreadProperties) null;
      }
    }

    public AkThreadProperties threadMonitor
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadMonitor_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
      }
      get
      {
        IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadMonitor_get(this.swigCPtr);
        return !(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : (AkThreadProperties) null;
      }
    }

    public uint uLEngineDefaultPoolSize
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_set(this.swigCPtr, value);
      }
      get
      {
        return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_get(this.swigCPtr);
      }
    }

    public float fLEngineDefaultPoolRatioThreshold
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_set(this.swigCPtr, value);
      }
      get
      {
        return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_get(this.swigCPtr);
      }
    }

    public ushort uNumRefillsInVoice
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uNumRefillsInVoice_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uNumRefillsInVoice_get(this.swigCPtr);
    }

    public uint uSampleRate
    {
      set => AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uSampleRate_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uSampleRate_get(this.swigCPtr);
    }

    public AkAudioAPI eAudioAPI
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_eAudioAPI_set(this.swigCPtr, (int) value);
      }
      get
      {
        return (AkAudioAPI) AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_eAudioAPI_get(this.swigCPtr);
      }
    }

    public bool bGlobalFocus
    {
      set
      {
        AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_bGlobalFocus_set(this.swigCPtr, value);
      }
      get => AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_bGlobalFocus_get(this.swigCPtr);
    }
  }

