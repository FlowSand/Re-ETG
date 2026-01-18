// Decompiled with JetBrains decompiler
// Type: GundromedaStrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class GundromedaStrain : PassiveItem
  {
    public float percentageHealthReduction = 0.1f;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      AIActor.HealthModifier *= Mathf.Clamp01(1f - this.percentageHealthReduction);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<GundromedaStrain>().m_pickedUpThisRun = true;
      AIActor.HealthModifier /= Mathf.Clamp01(1f - this.percentageHealthReduction);
      return debrisObject;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

