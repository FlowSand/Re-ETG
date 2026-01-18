// Decompiled with JetBrains decompiler
// Type: AkAudioDeviceState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public enum AkAudioDeviceState
  {
    AkDeviceState_Active = 1,
    AkDeviceState_Disabled = 2,
    AkDeviceState_NotPresent = 4,
    AkDeviceState_Unplugged = 8,
    AkDeviceState_All = 15, // 0x0000000F
  }

