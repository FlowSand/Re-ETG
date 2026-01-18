using System;

#nullable disable

public class AkMusicSettings : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkMusicSettings(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkMusicSettings()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkMusicSettings(), true)
        {
        }

        internal static IntPtr getCPtr(AkMusicSettings obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkMusicSettings() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkMusicSettings(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public float fStreamingLookAheadRatio
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_get(this.swigCPtr);
        }
    }

