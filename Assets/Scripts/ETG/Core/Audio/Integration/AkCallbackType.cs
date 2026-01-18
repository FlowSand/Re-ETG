#nullable disable

public enum AkCallbackType
    {
        AK_EndOfEvent = 1,
        AK_EndOfDynamicSequenceItem = 2,
        AK_Marker = 4,
        AK_Duration = 8,
        AK_SpeakerVolumeMatrix = 16, // 0x00000010
        AK_Starvation = 32, // 0x00000020
        AK_MusicPlaylistSelect = 64, // 0x00000040
        AK_MusicPlayStarted = 128, // 0x00000080
        AK_MusicSyncBeat = 256, // 0x00000100
        AK_MusicSyncBar = 512, // 0x00000200
        AK_MusicSyncEntry = 1024, // 0x00000400
        AK_MusicSyncExit = 2048, // 0x00000800
        AK_MusicSyncGrid = 4096, // 0x00001000
        AK_MusicSyncUserCue = 8192, // 0x00002000
        AK_MusicSyncPoint = 16384, // 0x00004000
        AK_MusicSyncAll = 32512, // 0x00007F00
        AK_MIDIEvent = 65536, // 0x00010000
        AK_CallbackBits = 1048575, // 0x000FFFFF
        AK_EnableGetSourcePlayPosition = 1048576, // 0x00100000
        AK_EnableGetMusicPlayPosition = 2097152, // 0x00200000
        AK_EnableGetSourceStreamBuffering = 4194304, // 0x00400000
        AK_Monitoring = 536870912, // 0x20000000
        AK_AudioInterruption = 570425344, // 0x22000000
        AK_AudioSourceChange = 587202560, // 0x23000000
        AK_Bank = 1073741824, // 0x40000000
    }

