using System;

#nullable disable

public class AkAudioSettings : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkAudioSettings(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkAudioSettings()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkAudioSettings(), true)
        {
        }

        internal static IntPtr getCPtr(AkAudioSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkAudioSettings() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkAudioSettings(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public uint uNumSamplesPerFrame
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkAudioSettings_uNumSamplesPerFrame_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkAudioSettings_uNumSamplesPerFrame_get(this.swigCPtr);
        }

        public uint uNumSamplesPerSecond
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkAudioSettings_uNumSamplesPerSecond_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkAudioSettings_uNumSamplesPerSecond_get(this.swigCPtr);
        }
    }

