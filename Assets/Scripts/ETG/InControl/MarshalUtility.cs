// Decompiled with JetBrains decompiler
// Type: InControl.MarshalUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace InControl
{
  public static class MarshalUtility
  {
    private static int[] buffer = new int[32 /*0x20*/];

    public static void Copy(IntPtr source, uint[] destination, int length)
    {
      Utility.ArrayExpand<int>(ref MarshalUtility.buffer, length);
      Marshal.Copy(source, MarshalUtility.buffer, 0, length);
      Buffer.BlockCopy((Array) MarshalUtility.buffer, 0, (Array) destination, 0, 4 * length);
    }
  }
}
