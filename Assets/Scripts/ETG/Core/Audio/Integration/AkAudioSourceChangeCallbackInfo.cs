using System;

#nullable disable

public class AkAudioSourceChangeCallbackInfo : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkAudioSourceChangeCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkAudioSourceChangeCallbackInfo()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkAudioSourceChangeCallbackInfo(), true)
        {
        }

        internal static IntPtr getCPtr(AkAudioSourceChangeCallbackInfo obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkAudioSourceChangeCallbackInfo() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkAudioSourceChangeCallbackInfo(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public bool bOtherAudioPlaying
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkAudioSourceChangeCallbackInfo_bOtherAudioPlaying_get(this.swigCPtr);
            }
        }
    }

