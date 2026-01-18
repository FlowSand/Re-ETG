using System;

#nullable disable

public class AkSoundPathInfoProxy : AkSoundPathInfo
    {
        private IntPtr swigCPtr;

        internal AkSoundPathInfoProxy(IntPtr cPtr, bool cMemoryOwn)
            : base(AkSoundEnginePINVOKE.CSharp_AkSoundPathInfoProxy_SWIGUpcast(cPtr), cMemoryOwn)
        {
            this.swigCPtr = cPtr;
        }

        public AkSoundPathInfoProxy()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkSoundPathInfoProxy(), true)
        {
        }

        internal static IntPtr getCPtr(AkSoundPathInfoProxy obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal override void setCPtr(IntPtr cPtr)
        {
            base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkSoundPathInfoProxy_SWIGUpcast(cPtr));
            this.swigCPtr = cPtr;
        }

        ~AkSoundPathInfoProxy() => this.Dispose();

        public override void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkSoundPathInfoProxy(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
                base.Dispose();
            }
        }

        public static int GetSizeOf() => AkSoundEnginePINVOKE.CSharp_AkSoundPathInfoProxy_GetSizeOf();

        public AkVector GetReflectionPoint(uint idx)
        {
            IntPtr reflectionPoint = AkSoundEnginePINVOKE.CSharp_AkSoundPathInfoProxy_GetReflectionPoint(this.swigCPtr, idx);
            return !(reflectionPoint == IntPtr.Zero) ? new AkVector(reflectionPoint, false) : (AkVector) null;
        }

        public AkTriangle GetTriangle(uint idx)
        {
            return new AkTriangle(AkSoundEnginePINVOKE.CSharp_AkSoundPathInfoProxy_GetTriangle(this.swigCPtr, idx), false);
        }
    }

