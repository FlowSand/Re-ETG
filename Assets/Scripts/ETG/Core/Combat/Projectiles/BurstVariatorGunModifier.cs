// Decompiled with JetBrains decompiler
// Type: BurstVariatorGunModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class BurstVariatorGunModifier : MonoBehaviour
  {
    public int NumDiceRolls = 2;
    public int DiceMin = 1;
    public int DiceMax = 6;
    private Gun m_gun;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.PostFired);
    }

    private int DiceRoll() => UnityEngine.Random.Range(this.DiceMin, this.DiceMax + 1);

    private void PostFired(PlayerController arg1, Gun arg2)
    {
      if (arg2.MidBurstFire)
        return;
      int num = 0;
      for (int index = 0; index < this.NumDiceRolls; ++index)
        num += this.DiceRoll();
      if ((UnityEngine.Object) arg2.RawSourceVolley != (UnityEngine.Object) null)
      {
        for (int index = 0; index < arg2.RawSourceVolley.projectiles.Count; ++index)
        {
          if (arg2.RawSourceVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Burst)
            arg2.RawSourceVolley.projectiles[index].burstShotCount = num;
        }
      }
      else if (arg2.singleModule.shootStyle == ProjectileModule.ShootStyle.Burst)
        arg2.singleModule.burstShotCount = num;
      if (!((UnityEngine.Object) arg2.modifiedVolley != (UnityEngine.Object) null))
        return;
      for (int index = 0; index < arg2.modifiedVolley.projectiles.Count; ++index)
      {
        if (arg2.modifiedVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Burst)
          arg2.modifiedVolley.projectiles[index].burstShotCount = num;
      }
    }
  }

