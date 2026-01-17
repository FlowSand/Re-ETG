// Decompiled with JetBrains decompiler
// Type: RandomStartingEquipmentSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
[Serializable]
public class RandomStartingEquipmentSettings
{
  public float D_CHANCE = 0.5f;
  public float C_CHANCE = 0.4f;
  public float B_CHANCE = 0.3f;
  public float A_CHANCE = 0.05f;
  public float S_CHANCE = 0.05f;
  [PickupIdentifier]
  public List<int> ExcludedPickups;
}
