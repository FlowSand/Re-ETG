// Decompiled with JetBrains decompiler
// Type: StoutBulletsItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class StoutBulletsItem : PassiveItem
  {
    public float RangeCap = 7f;
    public float MaxDamageIncrease = 1.75f;
    public float MinDamageIncrease = 1.125f;
    public float ScaleIncrease = 1.5f;
    public float DescaleAmount = 0.5f;
    public float DamageCutOnDescale = 2f;
    private PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      base.Pickup(player);
      player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
    }

    private void PostProcessBeam(BeamController obj)
    {
      if (!(bool) (UnityEngine.Object) obj)
        return;
      Projectile projectile = obj.projectile;
      if (!(bool) (UnityEngine.Object) projectile)
        return;
      this.PostProcessProjectile(projectile, 1f);
    }

    private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
    {
      float num = Mathf.Lerp(this.MinDamageIncrease, this.MaxDamageIncrease, Mathf.Clamp01(Mathf.Max(0.0f, obj.baseData.range - this.RangeCap) / 15f));
      obj.OnPostUpdate += new Action<Projectile>(this.HandlePostUpdate);
      obj.AdditionalScaleMultiplier *= this.ScaleIncrease;
      obj.baseData.damage *= num;
    }

    private void HandlePostUpdate(Projectile proj)
    {
      if (!(bool) (UnityEngine.Object) proj || (double) proj.GetElapsedDistance() <= (double) this.RangeCap)
        return;
      proj.RuntimeUpdateScale(this.DescaleAmount);
      proj.baseData.damage /= this.DamageCutOnDescale;
      proj.OnPostUpdate -= new Action<Projectile>(this.HandlePostUpdate);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      this.m_player = (PlayerController) null;
      debrisObject.GetComponent<StoutBulletsItem>().m_pickedUpThisRun = true;
      player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_player)
        return;
      this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
    }
  }

