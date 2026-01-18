using System;
using UnityEngine;

#nullable disable

public class MultiTemporaryOrbitalSynergyProcessor : MonoBehaviour
  {
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public GameObject OrbitalPrefab;
    private MultiTemporaryOrbitalLayer m_layer;
    private bool m_hasBeenInitialized;
    private Gun m_gun;
    private bool m_attached;
    private PlayerController m_lastPlayer;

    private void Start()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_layer = new MultiTemporaryOrbitalLayer();
      this.m_layer.collisionDamage = 3f;
    }

    private void Update()
    {
      if (!this.m_attached)
      {
        if ((bool) (UnityEngine.Object) this.m_gun && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner && this.m_gun.OwnerHasSynergy(this.RequiredSynergy))
        {
          PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
          currentOwner.OnAnyEnemyReceivedDamage += new Action<float, bool, HealthHaver>(this.HandleEnemyDamaged);
          this.m_lastPlayer = currentOwner;
          this.m_attached = true;
        }
      }
      else if (!(bool) (UnityEngine.Object) this.m_gun || !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !this.m_gun.OwnerHasSynergy(this.RequiredSynergy))
      {
        if ((bool) (UnityEngine.Object) this.m_lastPlayer)
          this.m_lastPlayer.OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.HandleEnemyDamaged);
        this.m_lastPlayer = (PlayerController) null;
        this.m_attached = false;
      }
      if (!this.m_hasBeenInitialized)
        return;
      this.m_layer.Update();
    }

    private void OnDestroy()
    {
      if (!this.m_attached)
        return;
      if (this.m_layer != null)
        this.m_layer.Disconnect();
      if ((bool) (UnityEngine.Object) this.m_lastPlayer)
        this.m_lastPlayer.OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.HandleEnemyDamaged);
      this.m_lastPlayer = (PlayerController) null;
      this.m_attached = false;
    }

    private void HandleEnemyDamaged(float dmg, bool fatal, HealthHaver target)
    {
      if (!(bool) (UnityEngine.Object) this.m_gun || !(this.m_gun.CurrentOwner is PlayerController) || !((UnityEngine.Object) (this.m_gun.CurrentOwner as PlayerController).CurrentGun == (UnityEngine.Object) this.m_gun) || !fatal)
        return;
      this.m_layer.targetNumberOrbitals = Mathf.Min(20, this.m_layer.targetNumberOrbitals + 1);
      if (this.m_hasBeenInitialized)
        return;
      this.m_layer.Initialize(this.m_lastPlayer, this.OrbitalPrefab);
      this.m_hasBeenInitialized = true;
    }
  }

