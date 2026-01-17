// Decompiled with JetBrains decompiler
// Type: GunExtraSettingSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GunExtraSettingSynergyProcessor : MonoBehaviour
    {
      public CustomSynergyType SynergyToCheck;
      public bool ChangesReflectedBulletDamage;
      public float ReflectedBulletDamageModifier = 1f;
      public bool ChangesReflectedBulletScale;
      public float ReflectedBulletScaleModifier = 1f;
      private Gun m_gun;

      private void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        if (this.ChangesReflectedBulletDamage)
          this.m_gun.OnReflectedBulletDamageModifier += new Func<float, float>(this.GetReflectedBulletDamageModifier);
        if (!this.ChangesReflectedBulletScale)
          return;
        this.m_gun.OnReflectedBulletScaleModifier += new Func<float, float>(this.GetReflectedBulletScaleModifier);
      }

      private float GetReflectedBulletScaleModifier(float inScale)
      {
        return this.HasSynergy() ? inScale * this.ReflectedBulletScaleModifier : inScale;
      }

      private bool HasSynergy()
      {
        return (bool) (UnityEngine.Object) this.m_gun && this.m_gun.CurrentOwner is PlayerController && (this.m_gun.CurrentOwner as PlayerController).HasActiveBonusSynergy(this.SynergyToCheck);
      }

      private float GetReflectedBulletDamageModifier(float inDamage)
      {
        return this.HasSynergy() ? inDamage * this.ReflectedBulletDamageModifier : inDamage;
      }
    }

}
