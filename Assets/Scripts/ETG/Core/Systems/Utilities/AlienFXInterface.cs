// Decompiled with JetBrains decompiler
// Type: AlienFXInterface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable

  public static class AlienFXInterface
  {
[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lpFileName);

[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);

[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

[DllImport("LightFX")]
    public static extern uint LFX_Initialize();

[DllImport("LightFX")]
    public static extern uint LFX_Update();

[DllImport("LightFX")]
    public static extern uint LFX_Reset();

[DllImport("LightFX")]
    public static extern uint LFX_Release();

[DllImport("LightFX", CallingConvention = CallingConvention.StdCall)]
    public static extern uint LFX_SetLightColor(uint p1, uint p2, ref AlienFXInterface._LFX_COLOR c);

[DllImport("LightFX")]
    public static extern uint LFX_GetNumDevices(ref uint numDevices);

[DllImport("LightFX")]
    public static extern uint LFX_GetNumLights(uint devIndex, ref uint numLights);

public struct _LFX_COLOR
    {
      public byte red;
      public byte green;
      public byte blue;
      public byte brightness;

      public _LFX_COLOR(Color32 combinedColor)
      {
        this.red = combinedColor.r;
        this.green = combinedColor.g;
        this.blue = combinedColor.b;
        this.brightness = combinedColor.a;
      }
    }
  }

