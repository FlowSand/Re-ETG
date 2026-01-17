// Decompiled with JetBrains decompiler
// Type: BurnableSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class BurnableSprite : MonoBehaviour
    {
      public float burnDuration = 2f;
      private GameObject burnParticleSystem;
      private bool m_isBurning;

      public void Initialize()
      {
        this.GetComponent<SpeculativeRigidbody>().OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
      }

      public void OnRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (this.m_isBurning || !((Object) rigidbodyCollision.OtherRigidbody.GetComponent<Projectile>() != (Object) null))
          return;
        this.Burn();
      }

      public void Burn()
      {
        this.m_isBurning = true;
        this.burnParticleSystem = SpawnManager.SpawnParticleSystem(BraveResources.Load<GameObject>("BurningSpriteEffect"));
        this.burnParticleSystem.transform.parent = this.transform;
        this.burnParticleSystem.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
        this.StartCoroutine(this.HandleBurning());
      }

      [DebuggerHidden]
      private IEnumerator HandleBurning()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BurnableSprite.<HandleBurning>c__Iterator0()
        {
          _this = this
        };
      }
    }

}
