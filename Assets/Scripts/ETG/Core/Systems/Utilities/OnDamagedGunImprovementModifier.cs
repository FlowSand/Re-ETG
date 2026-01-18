using System;
using UnityEngine;

#nullable disable

public class OnDamagedGunImprovementModifier : MonoBehaviour
  {
    public int AdditionalClipCapacity;
    private Gun m_gun;
    private PlayerController m_playerOwner;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnInitializedWithOwner += new Action<GameActor>(this.OnGunInitialized);
      this.m_gun.OnDropped += new System.Action(this.OnGunDroppedOrDestroyed);
      if (!((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null))
        return;
      this.OnGunInitialized(this.m_gun.CurrentOwner);
    }

    private void OnGunInitialized(GameActor obj)
    {
      if ((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null)
        this.OnGunDroppedOrDestroyed();
      if ((UnityEngine.Object) obj == (UnityEngine.Object) null || !(obj is PlayerController))
        return;
      this.m_playerOwner = obj as PlayerController;
      this.m_playerOwner.healthHaver.OnHealthChanged += new HealthHaver.OnHealthChangedEvent(this.OnHealthChanged);
    }

    private void OnHealthChanged(float resultValue, float maxValue)
    {
      this.m_gun.AdditionalClipCapacity = Mathf.FloorToInt((float) (((double) maxValue - (double) resultValue) * 2.0));
      this.m_playerOwner.stats.RecalculateStats(this.m_playerOwner);
    }

    private void OnDestroy() => this.OnGunDroppedOrDestroyed();

    private void OnGunDroppedOrDestroyed()
    {
      if (!((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null))
        return;
      this.m_playerOwner.healthHaver.OnHealthChanged -= new HealthHaver.OnHealthChangedEvent(this.OnHealthChanged);
      this.m_playerOwner = (PlayerController) null;
    }
  }

