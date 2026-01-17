// Decompiled with JetBrains decompiler
// Type: SpawnObjectOnRollItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class SpawnObjectOnRollItem : PassiveItem
    {
      public GameObject ObjectToSpawn;
      public bool DoBounce;
      public float BounceDuration = 1f;
      public float BounceStartVelocity = 5f;
      public float GravityAcceleration = 10f;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        player.OnRollStarted += new Action<PlayerController, Vector2>(this.OnRollStarted);
        base.Pickup(player);
      }

      private void OnRollStarted(PlayerController obj, Vector2 dirVec)
      {
        if (!(bool) (UnityEngine.Object) this.ObjectToSpawn)
          return;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ObjectToSpawn, obj.transform.position, Quaternion.identity);
        gameObject.GetComponent<tk2dSprite>().PlaceAtPositionByAnchor((Vector3) obj.CenterPosition, tk2dBaseSprite.Anchor.MiddleCenter);
        if (!this.DoBounce)
          return;
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleObjectBounce(gameObject.transform));
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        player.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
        debrisObject.GetComponent<SpawnObjectOnRollItem>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      [DebuggerHidden]
      private IEnumerator HandleObjectBounce(Transform target)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpawnObjectOnRollItem.<HandleObjectBounce>c__Iterator0()
        {
          target = target,
          _this = this
        };
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
          this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
        base.OnDestroy();
      }
    }

}
