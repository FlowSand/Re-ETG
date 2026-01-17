// Decompiled with JetBrains decompiler
// Type: InControl.InputControlType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace InControl
{
  public enum InputControlType
  {
    None = 0,
    LeftStickUp = 1,
    LeftStickDown = 2,
    LeftStickLeft = 3,
    LeftStickRight = 4,
    LeftStickButton = 5,
    RightStickUp = 6,
    RightStickDown = 7,
    RightStickLeft = 8,
    RightStickRight = 9,
    RightStickButton = 10, // 0x0000000A
    DPadUp = 11, // 0x0000000B
    DPadDown = 12, // 0x0000000C
    DPadLeft = 13, // 0x0000000D
    DPadRight = 14, // 0x0000000E
    LeftTrigger = 15, // 0x0000000F
    RightTrigger = 16, // 0x00000010
    LeftBumper = 17, // 0x00000011
    RightBumper = 18, // 0x00000012
    Action1 = 19, // 0x00000013
    Action2 = 20, // 0x00000014
    Action3 = 21, // 0x00000015
    Action4 = 22, // 0x00000016
    Action5 = 23, // 0x00000017
    Action6 = 24, // 0x00000018
    Action7 = 25, // 0x00000019
    Action8 = 26, // 0x0000001A
    Action9 = 27, // 0x0000001B
    Action10 = 28, // 0x0000001C
    Action11 = 29, // 0x0000001D
    Action12 = 30, // 0x0000001E
    Back = 100, // 0x00000064
    Start = 101, // 0x00000065
    Select = 102, // 0x00000066
    System = 103, // 0x00000067
    Options = 104, // 0x00000068
    Pause = 105, // 0x00000069
    Menu = 106, // 0x0000006A
    Share = 107, // 0x0000006B
    Home = 108, // 0x0000006C
    View = 109, // 0x0000006D
    Power = 110, // 0x0000006E
    Capture = 111, // 0x0000006F
    Plus = 112, // 0x00000070
    Minus = 113, // 0x00000071
    PedalLeft = 150, // 0x00000096
    PedalRight = 151, // 0x00000097
    PedalMiddle = 152, // 0x00000098
    GearUp = 153, // 0x00000099
    GearDown = 154, // 0x0000009A
    Pitch = 200, // 0x000000C8
    Roll = 201, // 0x000000C9
    Yaw = 202, // 0x000000CA
    ThrottleUp = 203, // 0x000000CB
    ThrottleDown = 204, // 0x000000CC
    ThrottleLeft = 205, // 0x000000CD
    ThrottleRight = 206, // 0x000000CE
    POVUp = 207, // 0x000000CF
    POVDown = 208, // 0x000000D0
    POVLeft = 209, // 0x000000D1
    POVRight = 210, // 0x000000D2
    TiltX = 250, // 0x000000FA
    TiltY = 251, // 0x000000FB
    TiltZ = 252, // 0x000000FC
    ScrollWheel = 253, // 0x000000FD
    [Obsolete("Use InputControlType.TouchPadButton instead.", true)] TouchPadTap = 254, // 0x000000FE
    TouchPadButton = 255, // 0x000000FF
    TouchPadXAxis = 256, // 0x00000100
    TouchPadYAxis = 257, // 0x00000101
    LeftSL = 258, // 0x00000102
    LeftSR = 259, // 0x00000103
    RightSL = 260, // 0x00000104
    RightSR = 261, // 0x00000105
    Command = 300, // 0x0000012C
    LeftStickX = 301, // 0x0000012D
    LeftStickY = 302, // 0x0000012E
    RightStickX = 303, // 0x0000012F
    RightStickY = 304, // 0x00000130
    DPadX = 305, // 0x00000131
    DPadY = 306, // 0x00000132
    Analog0 = 400, // 0x00000190
    Analog1 = 401, // 0x00000191
    Analog2 = 402, // 0x00000192
    Analog3 = 403, // 0x00000193
    Analog4 = 404, // 0x00000194
    Analog5 = 405, // 0x00000195
    Analog6 = 406, // 0x00000196
    Analog7 = 407, // 0x00000197
    Analog8 = 408, // 0x00000198
    Analog9 = 409, // 0x00000199
    Analog10 = 410, // 0x0000019A
    Analog11 = 411, // 0x0000019B
    Analog12 = 412, // 0x0000019C
    Analog13 = 413, // 0x0000019D
    Analog14 = 414, // 0x0000019E
    Analog15 = 415, // 0x0000019F
    Analog16 = 416, // 0x000001A0
    Analog17 = 417, // 0x000001A1
    Analog18 = 418, // 0x000001A2
    Analog19 = 419, // 0x000001A3
    Button0 = 500, // 0x000001F4
    Button1 = 501, // 0x000001F5
    Button2 = 502, // 0x000001F6
    Button3 = 503, // 0x000001F7
    Button4 = 504, // 0x000001F8
    Button5 = 505, // 0x000001F9
    Button6 = 506, // 0x000001FA
    Button7 = 507, // 0x000001FB
    Button8 = 508, // 0x000001FC
    Button9 = 509, // 0x000001FD
    Button10 = 510, // 0x000001FE
    Button11 = 511, // 0x000001FF
    Button12 = 512, // 0x00000200
    Button13 = 513, // 0x00000201
    Button14 = 514, // 0x00000202
    Button15 = 515, // 0x00000203
    Button16 = 516, // 0x00000204
    Button17 = 517, // 0x00000205
    Button18 = 518, // 0x00000206
    Button19 = 519, // 0x00000207
    Count = 520, // 0x00000208
  }
}
