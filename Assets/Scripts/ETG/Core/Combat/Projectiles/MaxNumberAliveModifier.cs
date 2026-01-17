// Decompiled with JetBrains decompiler
// Type: MaxNumberAliveModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class MaxNumberAliveModifier : MonoBehaviour
    {
      private List<Projectile> m_aliveProjectiles;
      public int MaxNumberAlive;

      private void Start()
      {
        Gun component = this.GetComponent<Gun>();
        this.m_aliveProjectiles = new List<Projectile>();
        component.PostProcessProjectile += new Action<Projectile>(this.HandleProjectileFired);
      }

      private void HandleProjectileFired(Projectile obj)
      {
        this.m_aliveProjectiles.Add(obj);
        this.CompactList();
      }

      private void CompactList()
      {
        for (int index = 0; index < this.m_aliveProjectiles.Count; ++index)
        {
          if (!(bool) (UnityEngine.Object) this.m_aliveProjectiles[index])
          {
            this.m_aliveProjectiles.RemoveAt(index);
            --index;
          }
        }
        while (this.m_aliveProjectiles.Count > this.MaxNumberAlive)
        {
          Projectile aliveProjectile = this.m_aliveProjectiles[0];
          this.m_aliveProjectiles.RemoveAt(0);
          aliveProjectile.DieInAir();
        }
      }
    }

}
