using System;

#nullable disable

public class AkImageSourceSettings : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkImageSourceSettings(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkImageSourceSettings()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkImageSourceSettings__SWIG_0(), true)
        {
        }

        public AkImageSourceSettings(
            AkVector in_sourcePosition,
            float in_fDistanceScalingFactor,
            float in_fLevel)
            : this(AkSoundEnginePINVOKE.CSharp_new_AkImageSourceSettings__SWIG_1(AkVector.getCPtr(in_sourcePosition), in_fDistanceScalingFactor, in_fLevel), true)
        {
        }

        internal static IntPtr getCPtr(AkImageSourceSettings obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkImageSourceSettings() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkImageSourceSettings(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public void SetOneTexture(uint in_texture)
        {
            AkSoundEnginePINVOKE.CSharp_AkImageSourceSettings_SetOneTexture(this.swigCPtr, in_texture);
        }

        public void SetName(string in_pName)
        {
            AkSoundEnginePINVOKE.CSharp_AkImageSourceSettings_SetName(this.swigCPtr, in_pName);
        }

        public AkImageSourceParams params_
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkImageSourceSettings_params__set(this.swigCPtr, AkImageSourceParams.getCPtr(value));
            }
            get
            {
                IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkImageSourceSettings_params__get(this.swigCPtr);
                return !(cPtr == IntPtr.Zero) ? new AkImageSourceParams(cPtr, false) : (AkImageSourceParams) null;
            }
        }
    }

