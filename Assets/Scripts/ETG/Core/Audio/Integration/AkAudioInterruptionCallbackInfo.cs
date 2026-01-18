using System;

#nullable disable

public class AkAudioInterruptionCallbackInfo : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkAudioInterruptionCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkAudioInterruptionCallbackInfo()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkAudioInterruptionCallbackInfo(), true)
        {
        }

        internal static IntPtr getCPtr(AkAudioInterruptionCallbackInfo obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkAudioInterruptionCallbackInfo() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkAudioInterruptionCallbackInfo(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public bool bEnterInterruption
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkAudioInterruptionCallbackInfo_bEnterInterruption_get(this.swigCPtr);
            }
        }
    }

