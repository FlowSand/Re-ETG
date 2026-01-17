// Decompiled with JetBrains decompiler
// Type: AkMeteringFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Audio.Integration
{
    public enum AkMeteringFlags
    {
      AK_NoMetering = 0,
      AK_EnableBusMeter_Peak = 1,
      AK_EnableBusMeter_TruePeak = 2,
      AK_EnableBusMeter_RMS = 4,
      AK_EnableBusMeter_KPower = 16, // 0x00000010
    }

}
