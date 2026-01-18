using System;

#nullable disable

public class _ArrayPoolDefault : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal _ArrayPoolDefault(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public _ArrayPoolDefault()
            : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolDefault(), true)
        {
        }

        internal static IntPtr getCPtr(_ArrayPoolDefault obj) => obj == null ? IntPtr.Zero : obj.swigCPtr;

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~_ArrayPoolDefault() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolDefault(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public static int Get() => AkSoundEnginePINVOKE.CSharp__ArrayPoolDefault_Get();
    }

