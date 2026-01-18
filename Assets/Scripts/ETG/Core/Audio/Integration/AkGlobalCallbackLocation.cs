#nullable disable

public enum AkGlobalCallbackLocation
    {
        AkGlobalCallbackLocation_Register = 1,
        AkGlobalCallbackLocation_Begin = 2,
        AkGlobalCallbackLocation_PreProcessMessageQueueForRender = 4,
        AkGlobalCallbackLocation_PostMessagesProcessed = 8,
        AkGlobalCallbackLocation_Num = 9,
        AkGlobalCallbackLocation_BeginRender = 16, // 0x00000010
        AkGlobalCallbackLocation_EndRender = 32, // 0x00000020
        AkGlobalCallbackLocation_End = 64, // 0x00000040
        AkGlobalCallbackLocation_Term = 128, // 0x00000080
        AkGlobalCallbackLocation_Monitor = 256, // 0x00000100
    }

