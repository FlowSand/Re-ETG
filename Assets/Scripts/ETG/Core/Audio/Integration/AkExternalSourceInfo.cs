using System;

#nullable disable

public class AkExternalSourceInfo : IDisposable
    {
        private IntPtr swigCPtr;
        protected bool swigCMemOwn;

        internal AkExternalSourceInfo(IntPtr cPtr, bool cMemoryOwn)
        {
            this.swigCMemOwn = cMemoryOwn;
            this.swigCPtr = cPtr;
        }

        public AkExternalSourceInfo()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_0(), true)
        {
        }

        public AkExternalSourceInfo(
            IntPtr in_pInMemory,
            uint in_uiMemorySize,
            uint in_iExternalSrcCookie,
            uint in_idCodec)
            : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_1(in_pInMemory, in_uiMemorySize, in_iExternalSrcCookie, in_idCodec), true)
        {
        }

        public AkExternalSourceInfo(string in_pszFileName, uint in_iExternalSrcCookie, uint in_idCodec)
            : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_2(in_pszFileName, in_iExternalSrcCookie, in_idCodec), true)
        {
        }

        public AkExternalSourceInfo(uint in_idFile, uint in_iExternalSrcCookie, uint in_idCodec)
            : this(AkSoundEnginePINVOKE.CSharp_new_AkExternalSourceInfo__SWIG_3(in_idFile, in_iExternalSrcCookie, in_idCodec), true)
        {
        }

        internal static IntPtr getCPtr(AkExternalSourceInfo obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal virtual void setCPtr(IntPtr cPtr)
        {
            this.Dispose();
            this.swigCPtr = cPtr;
        }

        ~AkExternalSourceInfo() => this.Dispose();

        public virtual void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkExternalSourceInfo(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
            }
        }

        public uint iExternalSrcCookie
        {
            set
            {
                AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_iExternalSrcCookie_set(this.swigCPtr, value);
            }
            get => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_iExternalSrcCookie_get(this.swigCPtr);
        }

        public uint idCodec
        {
            set => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idCodec_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idCodec_get(this.swigCPtr);
        }

        public string szFile
        {
            set => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_szFile_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_szFile_get(this.swigCPtr);
        }

        public IntPtr pInMemory
        {
            set => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_pInMemory_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_pInMemory_get(this.swigCPtr);
        }

        public uint uiMemorySize
        {
            set => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_uiMemorySize_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_uiMemorySize_get(this.swigCPtr);
        }

        public uint idFile
        {
            set => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idFile_set(this.swigCPtr, value);
            get => AkSoundEnginePINVOKE.CSharp_AkExternalSourceInfo_idFile_get(this.swigCPtr);
        }
    }

