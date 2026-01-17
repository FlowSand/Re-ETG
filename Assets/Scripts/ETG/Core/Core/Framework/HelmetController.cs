// Decompiled with JetBrains decompiler
// Type: HelmetController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HelmetController : BraveBehaviour
    {
      public GameObject helmetEffect;
      public float helmetForce = 5f;

      public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);

      protected override void OnDestroy()
      {
        this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
        base.OnDestroy();
      }

      public void OnPreDeath(Vector2 finalDamageDirection)
      {
        if (this.aiActor.IsFalling || !((UnityEngine.Object) this.helmetEffect != (UnityEngine.Object) null))
          return;
        DebrisObject component = SpawnManager.SpawnDebris(this.helmetEffect, (Vector3) this.specRigidbody.UnitTopLeft, Quaternion.identity).GetComponent<DebrisObject>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        component.Trigger((Vector3) (finalDamageDirection.normalized * this.helmetForce), 1f);
      }
    }

}
