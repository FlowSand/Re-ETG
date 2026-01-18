using System;

#nullable disable

public class AkVector : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkVector(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkVector()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkVector(), true)
        {
        }

        internal static IntPtr getCPtr(AkVector obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkVector() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkVector(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public void Zero() => AkSoundEnginePINVOKE.CSharp_AkVector_Zero(this.swigCPtr);

        public float X
        {
            set => AkSoundEnginePINVOKE.CSharp_AkVector_X_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkVector_X_get(this.swigCPtr);
        }

        public float Y
        {
            set => AkSoundEnginePINVOKE.CSharp_AkVector_Y_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkVector_Y_get(this.swigCPtr);
        }

        public float Z
        {
            set => AkSoundEnginePINVOKE.CSharp_AkVector_Z_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkVector_Z_get(this.swigCPtr);
        }
    }

