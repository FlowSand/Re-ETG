using System;

#nullable disable

public class AkSegmentInfo : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkSegmentInfo(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkSegmentInfo()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkSegmentInfo(), true)
        {
        }

        internal static IntPtr getCPtr(AkSegmentInfo obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkSegmentInfo() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkSegmentInfo(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public int iCurrentPosition
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iCurrentPosition_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iCurrentPosition_get(this.swigCPtr);
        }

        public int iPreEntryDuration
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPreEntryDuration_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPreEntryDuration_get(this.swigCPtr);
        }

        public int iActiveDuration
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iActiveDuration_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iActiveDuration_get(this.swigCPtr);
        }

        public int iPostExitDuration
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPostExitDuration_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iPostExitDuration_get(this.swigCPtr);
        }

        public int iRemainingLookAheadTime
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iRemainingLookAheadTime_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_iRemainingLookAheadTime_get(this.swigCPtr);
        }

        public float fBeatDuration
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fBeatDuration_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fBeatDuration_get(this.swigCPtr);
        }

        public float fBarDuration
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fBarDuration_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fBarDuration_get(this.swigCPtr);
        }

        public float fGridDuration
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fGridDuration_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fGridDuration_get(this.swigCPtr);
        }

        public float fGridOffset
        {
            set => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fGridOffset_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkSegmentInfo_fGridOffset_get(this.swigCPtr);
        }
    }

