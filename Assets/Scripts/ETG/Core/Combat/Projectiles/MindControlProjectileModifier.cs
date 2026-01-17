// Decompiled with JetBrains decompiler
// Type: MindControlProjectileModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class MindControlProjectileModifier : MonoBehaviour
    {
      private void Start()
      {
        Projectile component = this.GetComponent<Projectile>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        component.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
      }

      private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
      {
        if (!(bool) (UnityEngine.Object) arg2 || !(bool) (UnityEngine.Object) arg2.aiActor)
          return;
        AIActor aiActor = arg2.aiActor;
        if (!aiActor.IsNormalEnemy || aiActor.healthHaver.IsBoss || aiActor.IsHarmlessEnemy || (bool) (UnityEngine.Object) aiActor.gameObject.GetComponent<MindControlEffect>())
          return;
        aiActor.gameObject.GetOrAddComponent<MindControlEffect>().owner = arg1.Owner as PlayerController;
      }
    }

}
