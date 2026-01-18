using System;

#nullable disable

public class AkChannelConfig : IDisposable
  {
    private IntPtr swigCPtr;
    protected bool swigCMemOwn;

    internal AkChannelConfig(IntPtr cPtr, bool cMemoryOwn)
    {
      this.swigCMemOwn = cMemoryOwn;
      this.swigCPtr = cPtr;
    }

    public AkChannelConfig()
      : this(AkSoundEnginePINVOKE.CSharp_new_AkChannelConfig__SWIG_0(), true)
    {
    }

    public AkChannelConfig(uint in_uNumChannels, uint in_uChannelMask)
      : this(AkSoundEnginePINVOKE.CSharp_new_AkChannelConfig__SWIG_1(in_uNumChannels, in_uChannelMask), true)
    {
    }

    internal static IntPtr getCPtr(AkChannelConfig obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

    internal virtual void setCPtr(IntPtr cPtr)
    {
      this.Dispose();
      this.swigCPtr = cPtr;
    }

    ~AkChannelConfig() => this.Dispose();

    public virtual void Dispose()
    {
      lock ((object) this)
      {
        if (this.swigCPtr != IntPtr.Zero)
        {
          if (this.swigCMemOwn)
          {
            this.swigCMemOwn = false;
            AkSoundEnginePINVOKE.CSharp_delete_AkChannelConfig(this.swigCPtr);
          }
          this.swigCPtr = IntPtr.Zero;
        }
        GC.SuppressFinalize((object) this);
      }
    }

    public uint uNumChannels
    {
      set => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_uNumChannels_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_uNumChannels_get(this.swigCPtr);
    }

    public uint eConfigType
    {
      set => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_eConfigType_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_eConfigType_get(this.swigCPtr);
    }

    public uint uChannelMask
    {
      set => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_uChannelMask_set(this.swigCPtr, value);
      get => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_uChannelMask_get(this.swigCPtr);
    }

    public void Clear() => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_Clear(this.swigCPtr);

    public void SetStandard(uint in_uChannelMask)
    {
      AkSoundEnginePINVOKE.CSharp_AkChannelConfig_SetStandard(this.swigCPtr, in_uChannelMask);
    }

    public void SetStandardOrAnonymous(uint in_uNumChannels, uint in_uChannelMask)
    {
      AkSoundEnginePINVOKE.CSharp_AkChannelConfig_SetStandardOrAnonymous(this.swigCPtr, in_uNumChannels, in_uChannelMask);
    }

    public void SetAnonymous(uint in_uNumChannels)
    {
      AkSoundEnginePINVOKE.CSharp_AkChannelConfig_SetAnonymous(this.swigCPtr, in_uNumChannels);
    }

    public void SetAmbisonic(uint in_uNumChannels)
    {
      AkSoundEnginePINVOKE.CSharp_AkChannelConfig_SetAmbisonic(this.swigCPtr, in_uNumChannels);
    }

    public bool IsValid() => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_IsValid(this.swigCPtr);

    public uint Serialize() => AkSoundEnginePINVOKE.CSharp_AkChannelConfig_Serialize(this.swigCPtr);

    public void Deserialize(uint in_uChannelConfig)
    {
      AkSoundEnginePINVOKE.CSharp_AkChannelConfig_Deserialize(this.swigCPtr, in_uChannelConfig);
    }

    public AkChannelConfig RemoveLFE()
    {
      return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_AkChannelConfig_RemoveLFE(this.swigCPtr), true);
    }

    public AkChannelConfig RemoveCenter()
    {
      return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_AkChannelConfig_RemoveCenter(this.swigCPtr), true);
    }

    public bool IsChannelConfigSupported()
    {
      return AkSoundEnginePINVOKE.CSharp_AkChannelConfig_IsChannelConfigSupported(this.swigCPtr);
    }
  }

