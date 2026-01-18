using System;

#nullable disable

public class AkMusicPlaylistCallbackInfo : AkEventCallbackInfo
    {
        private IntPtr swigCPtr;

        internal AkMusicPlaylistCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
            : base(AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_SWIGUpcast(cPtr), cMemoryOwn)
        {
            this.swigCPtr = cPtr;
        }

        public AkMusicPlaylistCallbackInfo()
            : this(AkSoundEnginePINVOKE.CSharp_new_AkMusicPlaylistCallbackInfo(), true)
        {
        }

        internal static IntPtr getCPtr(AkMusicPlaylistCallbackInfo obj)
        {
            return obj == null ? IntPtr.Zero : obj.swigCPtr;
        }

        internal override void setCPtr(IntPtr cPtr)
        {
            base.setCPtr(AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_SWIGUpcast(cPtr));
            this.swigCPtr = cPtr;
        }

        ~AkMusicPlaylistCallbackInfo() => this.Dispose();

        public override void Dispose()
        {
            lock ((object) this)
            {
                if (this.swigCPtr != IntPtr.Zero)
                {
                    if (this.swigCMemOwn)
                    {
                        this.swigCMemOwn = false;
                        AkSoundEnginePINVOKE.CSharp_delete_AkMusicPlaylistCallbackInfo(this.swigCPtr);
                    }
                    this.swigCPtr = IntPtr.Zero;
                }
                GC.SuppressFinalize((object) this);
                base.Dispose();
            }
        }

        public uint playlistID
        {
            get => AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_playlistID_get(this.swigCPtr);
        }

        public uint uNumPlaylistItems
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_get(this.swigCPtr);
            }
        }

        public uint uPlaylistSelection
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_get(this.swigCPtr);
            }
        }

        public uint uPlaylistItemDone
        {
            get
            {
                return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_get(this.swigCPtr);
            }
        }
    }

