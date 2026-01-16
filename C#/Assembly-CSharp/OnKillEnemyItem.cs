// Decompiled with JetBrains decompiler
// Type: OnKillEnemyItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class OnKillEnemyItem : PassiveItem
{
  public OnKillEnemyItem.ActivationStyle activationStyle;
  [ShowInInspectorIf("activationStyle", 0, false)]
  public float chanceOfActivating = 1f;
  [ShowInInspectorIf("activationStyle", 1, false)]
  public int numEnemiesBeforeActivation = 3;
  public int ammoToGain = 1;
  public float healthToGain;
  private int m_activations;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    player.OnKilledEnemy += new Action<PlayerController>(this.OnKilledEnemy);
  }

  public void OnKilledEnemy(PlayerController source)
  {
    ++this.m_activations;
    if (this.activationStyle == OnKillEnemyItem.ActivationStyle.RANDOM_CHANCE)
    {
      if ((double) UnityEngine.Random.value >= (double) this.chanceOfActivating)
        return;
      this.DoEffect(source);
    }
    else
    {
      if (this.activationStyle != OnKillEnemyItem.ActivationStyle.EVERY_X_ENEMIES || this.m_activations % this.numEnemiesBeforeActivation != 0)
        return;
      this.DoEffect(source);
    }
  }

  private void DoEffect(PlayerController source)
  {
    if (this.ammoToGain > 0 && (UnityEngine.Object) source.CurrentGun != (UnityEngine.Object) null)
      source.CurrentGun.GainAmmo(Mathf.Max(1, (int) ((double) this.ammoToGain * 0.0099999997764825821 * (double) source.CurrentGun.AdjustedMaxAmmo)));
    if ((double) this.healthToGain <= 0.0)
      return;
    source.healthHaver.ApplyHealing(this.healthToGain);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    debrisObject.GetComponent<OnKillEnemyItem>().m_pickedUpThisRun = true;
    player.OnKilledEnemy -= new Action<PlayerController>(this.OnKilledEnemy);
    return debrisObject;
  }

  protected override void OnDestroy() => base.OnDestroy();

  public enum ActivationStyle
  {
    RANDOM_CHANCE,
    EVERY_X_ENEMIES,
  }
}
