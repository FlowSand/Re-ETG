using UnityEngine;

#nullable disable

public class AmmoRegenSynergyProcessor : MonoBehaviour
  {
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public float AmmoPerSecond = 0.1f;
    public bool PreventGainWhileFiring = true;
    private Gun m_gun;
    private float m_ammoCounter;
    private float m_gameTimeOnDisable;

    private void Awake() => this.m_gun = this.GetComponent<Gun>();

    private void Update()
    {
      if (!(bool) (Object) this.m_gun.CurrentOwner || !this.m_gun.OwnerHasSynergy(this.RequiredSynergy) || this.PreventGainWhileFiring && this.m_gun.IsFiring)
        return;
      this.m_ammoCounter += BraveTime.DeltaTime * this.AmmoPerSecond;
      if ((double) this.m_ammoCounter <= 1.0)
        return;
      int amt = Mathf.FloorToInt(this.m_ammoCounter);
      this.m_ammoCounter -= (float) amt;
      this.m_gun.GainAmmo(amt);
    }

    private void OnEnable()
    {
      if ((double) this.m_gameTimeOnDisable <= 0.0)
        return;
      this.m_ammoCounter += (UnityEngine.Time.time - this.m_gameTimeOnDisable) * this.AmmoPerSecond;
      this.m_gameTimeOnDisable = 0.0f;
    }

    private void OnDisable() => this.m_gameTimeOnDisable = UnityEngine.Time.time;
  }

