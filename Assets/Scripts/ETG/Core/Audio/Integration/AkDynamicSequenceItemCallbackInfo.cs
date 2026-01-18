using System;

#nullable disable

public class AkDynamicSequenceItemCallbackInfo : AkCallbackInfo
    {
        private IntPtr swigCPtr;

        internal AkDynamicSequenceItemCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
            : base(AkSoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_SWIGUpcast(cPtr), cMemoryOwn)
        {
            this.swigCPtr = cPtr;
        }

        public AkDynamicSequenceItemCallbackInfo()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkDynamicSequenceItemCallbackInfo(), true)
        {
        }

        internal static IntPtr getCPtr(AkDynamicSequenceItemCallbackInfo obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal override void setCPtr(IntPtr cPtr)
        {
            base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_SWIGUpcast(cPtr));
            this.swigCPtr = cPtr;
        }

        ~AkDynamicSequenceItemCallbackInfo() => this.Dispose();

        public override void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkDynamicSequenceItemCallbackInfo(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
                base.Dispose();
            }
        }

        public uint playingID
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_playingID_get(this.swigCPtr);
            }
        }

        public uint audioNodeID
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_audioNodeID_get(this.swigCPtr);
            }
        }

        public IntPtr pCustomInfo
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkDynamicSequenceItemCallbackInfo_pCustomInfo_get(this.swigCPtr);
            }
        }
    }

