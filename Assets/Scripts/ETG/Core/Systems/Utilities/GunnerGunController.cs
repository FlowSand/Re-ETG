// Decompiled with JetBrains decompiler
// Type: GunnerGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GunnerGunController : MonoBehaviour
    {
      public float ChanceToSkull = 1f;
      public GameObject SkullPrefab;
      public float Lifespan = 4f;
      public int AmmoLossOnDamage;
      public float ChanceToTriggerSynergy = 0.25f;
      private Gun m_gun;
      private bool m_initialized;
      private PlayerController m_player;
      private GameObject m_extantSkull;

      private void Awake() => this.m_gun = this.GetComponent<Gun>();

      private void HandleReceivedDamage(PlayerController p)
      {
        float num = 0.0f;
        bool flag = (bool) (UnityEngine.Object) p.CurrentGun && (UnityEngine.Object) p.CurrentGun == (UnityEngine.Object) this.m_gun;
        if (flag)
          num = this.ChanceToSkull;
        if ((bool) (UnityEngine.Object) p && p.HasActiveBonusSynergy(CustomSynergyType.WHALE_OF_A_TIME))
          num = this.ChanceToTriggerSynergy;
        if ((double) UnityEngine.Random.value < (double) num && !(bool) (UnityEngine.Object) this.m_extantSkull && (!flag || this.m_gun.ammo >= this.AmmoLossOnDamage && this.m_gun.ammo > 0))
        {
          if (flag)
            this.m_gun.LoseAmmo(this.AmmoLossOnDamage);
          this.m_extantSkull = SpawnManager.SpawnDebris(this.SkullPrefab, p.CenterPosition.ToVector3ZisY(), Quaternion.identity);
          DebrisObject component1 = this.m_extantSkull.GetComponent<DebrisObject>();
          component1.FlagAsPickup();
          component1.Trigger((UnityEngine.Random.insideUnitCircle.normalized * 20f).ToVector3ZUp(3f), 1f, 0.0f);
          SpeculativeRigidbody component2 = this.m_extantSkull.GetComponent<SpeculativeRigidbody>();
          PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(component2);
          component2.RegisterTemporaryCollisionException(p.specRigidbody, 0.25f);
          component2.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleSkullTrigger);
          component2.StartCoroutine(this.HandleLifespan(this.m_extantSkull));
        }
        else
        {
          if (!(bool) (UnityEngine.Object) this.m_extantSkull)
            return;
          LootEngine.DoDefaultPurplePoof((Vector2) (this.m_extantSkull.transform.position + new Vector3(0.75f, 0.5f, 0.0f)));
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantSkull.gameObject);
          this.m_extantSkull = (GameObject) null;
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleLifespan(GameObject source)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunnerGunController__HandleLifespanc__Iterator0()
        {
          source = source,
          _this = this
        };
      }

      private void HandleSkullTrigger(
        SpeculativeRigidbody specRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (!(bool) (UnityEngine.Object) specRigidbody)
          return;
        PlayerController component = specRigidbody.GetComponent<PlayerController>();
        if (!(bool) (UnityEngine.Object) component || (!(bool) (UnityEngine.Object) component.CurrentGun || !((UnityEngine.Object) component.CurrentGun == (UnityEngine.Object) this.m_gun)) && !component.HasActiveBonusSynergy(CustomSynergyType.WHALE_OF_A_TIME))
          return;
        sourceSpecRigidbody.OnEnterTrigger -= new SpeculativeRigidbody.OnTriggerDelegate(this.HandleSkullTrigger);
        this.m_extantSkull.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("gonner_skull_pickup_vfx");
        this.m_extantSkull = (GameObject) null;
        if (component.characterIdentity == PlayableCharacters.Robot)
        {
          ++component.healthHaver.Armor;
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
          GameObject effect = BraveResources.Load<GameObject>("Global VFX/VFX_Healing_Sparkles_001");
          if (!((UnityEngine.Object) effect != (UnityEngine.Object) null))
            return;
          component.PlayEffectOnActor(effect, Vector3.zero);
        }
        else
        {
          component.healthHaver.ApplyHealing(0.5f);
          int num = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
          GameObject effect = BraveResources.Load<GameObject>("Global VFX/VFX_Healing_Sparkles_001");
          if (!((UnityEngine.Object) effect != (UnityEngine.Object) null))
            return;
          component.PlayEffectOnActor(effect, Vector3.zero);
        }
      }

      private void Update()
      {
        if (this.m_initialized && (!(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || (UnityEngine.Object) this.m_gun.CurrentOwner.CurrentGun != (UnityEngine.Object) this.m_gun))
        {
          this.Disengage();
        }
        else
        {
          if (this.m_initialized || !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !((UnityEngine.Object) this.m_gun.CurrentOwner.CurrentGun == (UnityEngine.Object) this.m_gun))
            return;
          this.Engage();
        }
      }

      private void OnDestroy() => this.Disengage();

      private void Engage()
      {
        this.m_initialized = true;
        this.m_player = this.m_gun.CurrentOwner as PlayerController;
        this.m_player.OnReceivedDamage += new Action<PlayerController>(this.HandleReceivedDamage);
      }

      private void Disengage()
      {
        if ((bool) (UnityEngine.Object) this.m_player)
          this.m_player.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
        if ((bool) (UnityEngine.Object) this.m_extantSkull)
        {
          LootEngine.DoDefaultPurplePoof((Vector2) (this.m_extantSkull.transform.position + new Vector3(0.75f, 0.5f, 0.0f)));
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantSkull.gameObject);
          this.m_extantSkull = (GameObject) null;
        }
        this.m_player = (PlayerController) null;
        this.m_initialized = false;
      }
    }

}
