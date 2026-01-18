using System;

#nullable disable

public class AkDeviceSettings : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkDeviceSettings(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkDeviceSettings()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkDeviceSettings(), true)
        {
        }

        internal static IntPtr getCPtr(AkDeviceSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkDeviceSettings() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkDeviceSettings(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public IntPtr pIOMemory
        {
            set => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_pIOMemory_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_pIOMemory_get(this.swigCPtr);
        }

        public uint uIOMemorySize
        {
            set => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemorySize_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemorySize_get(this.swigCPtr);
        }

        public uint uIOMemoryAlignment
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemoryAlignment_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemoryAlignment_get(this.swigCPtr);
        }

        public int ePoolAttributes
        {
            set => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_ePoolAttributes_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_ePoolAttributes_get(this.swigCPtr);
        }

        public uint uGranularity
        {
            set => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uGranularity_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uGranularity_get(this.swigCPtr);
        }

        public uint uSchedulerTypeFlags
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uSchedulerTypeFlags_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uSchedulerTypeFlags_get(this.swigCPtr);
        }

        public AkThreadProperties threadProperties
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_threadProperties_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
            }
            get
            {
                IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_threadProperties_get(this.swigCPtr);
                return !(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : (AkThreadProperties) null;
            }
        }

        public float fTargetAutoStmBufferLength
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_set(this.swigCPtr, value);
            }
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_get(this.swigCPtr);
            }
        }

        public uint uMaxConcurrentIO
        {
            set => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxConcurrentIO_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxConcurrentIO_get(this.swigCPtr);
        }

        public bool bUseStreamCache
        {
            set => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_bUseStreamCache_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_bUseStreamCache_get(this.swigCPtr);
        }

        public uint uMaxCachePinnedBytes
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxCachePinnedBytes_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxCachePinnedBytes_get(this.swigCPtr);
        }
    }

