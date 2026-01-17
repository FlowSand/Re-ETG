// Decompiled with JetBrains decompiler
// Type: BloodthirstSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class BloodthirstSettings
    {
      public int NumKillsForHealRequiredBase = 5;
      public int NumKillsAddedPerHealthGained = 5;
      public int NumKillsRequiredCap = 50;
      public float Radius = 5f;
      public float DamagePerSecond = 30f;
      [Range(0.0f, 1f)]
      public float PercentAffected = 0.5f;
    }

}
