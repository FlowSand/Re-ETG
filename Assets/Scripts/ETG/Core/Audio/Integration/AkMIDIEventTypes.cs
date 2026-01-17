// Decompiled with JetBrains decompiler
// Type: AkMIDIEventTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public enum AkMIDIEventTypes
    {
      NOTE_OFF = 128, // 0x00000080
      NOTE_ON = 144, // 0x00000090
      NOTE_AFTERTOUCH = 160, // 0x000000A0
      CONTROLLER = 176, // 0x000000B0
      PROGRAM_CHANGE = 192, // 0x000000C0
      CHANNEL_AFTERTOUCH = 208, // 0x000000D0
      PITCH_BEND = 224, // 0x000000E0
      SYSEX = 240, // 0x000000F0
      ESCAPE = 247, // 0x000000F7
      META = 255, // 0x000000FF
    }

}
