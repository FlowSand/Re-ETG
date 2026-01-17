// Decompiled with JetBrains decompiler
// Type: SCEPadWrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;

#nullable disable
public class SCEPadWrapper
{
  private const int SCE_PAD_MAX_TOUCH_NUM = 2;
  private const int SCE_PAD_MAX_DEVICE_UNIQUE_DATA_SIZE = 12;
  public const int SCE_OK = 0;

  [DllImport("PS4NativePad")]
  public static extern int PadReadState(int handle, out SCEPadWrapper.ScePadData pData);

  public enum ScePadButtonDataOffset : uint
  {
    SCE_PAD_BUTTON_L3 = 2,
    SCE_PAD_BUTTON_R3 = 4,
    SCE_PAD_BUTTON_OPTIONS = 8,
    SCE_PAD_BUTTON_UP = 16, // 0x00000010
    SCE_PAD_BUTTON_RIGHT = 32, // 0x00000020
    SCE_PAD_BUTTON_DOWN = 64, // 0x00000040
    SCE_PAD_BUTTON_LEFT = 128, // 0x00000080
    SCE_PAD_BUTTON_L2 = 256, // 0x00000100
    SCE_PAD_BUTTON_R2 = 512, // 0x00000200
    SCE_PAD_BUTTON_L1 = 1024, // 0x00000400
    SCE_PAD_BUTTON_R1 = 2048, // 0x00000800
    SCE_PAD_BUTTON_TRIANGLE = 4096, // 0x00001000
    SCE_PAD_BUTTON_CIRCLE = 8192, // 0x00002000
    SCE_PAD_BUTTON_CROSS = 16384, // 0x00004000
    SCE_PAD_BUTTON_SQUARE = 32768, // 0x00008000
    SCE_PAD_BUTTON_TOUCH_PAD = 1048576, // 0x00100000
    SCE_PAD_BUTTON_INTERCEPTED = 2147483648, // 0x80000000
  }

  public struct ScePadAnalogStick
  {
    public byte x;
    public byte y;
  }

  public struct ScePadAnalogButtons
  {
    public byte l2;
    public byte r2;
    private byte pad1;
    private byte pad2;
  }

  public struct ScePadTouch
  {
    private ushort x;
    private ushort y;
    private byte id;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    private byte[] reserve;
  }

  public struct ScePadTouchData
  {
    private byte touchNum;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
    private byte[] reserve;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    private SCEPadWrapper.ScePadTouch[] touch;
  }

  public struct ScePadExtensionUnitData
  {
    private uint extensionUnitId;
    private byte reserve;
    private byte dataLength;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    private byte[] data;
  }

  public struct SceFQuaternion
  {
    private float x;
    private float y;
    private float z;
    private float w;
  }

  public struct SceFVector3
  {
    private float x;
    private float y;
    private float z;
  }

  public struct ScePadData
  {
    public uint buttons;
    public SCEPadWrapper.ScePadAnalogStick leftStick;
    public SCEPadWrapper.ScePadAnalogStick rightStick;
    public SCEPadWrapper.ScePadAnalogButtons analogButtons;
    public bool connected;
  }
}
