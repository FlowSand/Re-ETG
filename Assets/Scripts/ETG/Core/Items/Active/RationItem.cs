using UnityEngine;

#nullable disable

public class RationItem : PlayerItem
  {
    public float healingAmount = 2f;
    public GameObject healVFX;

    protected override void DoEffect(PlayerController user)
    {
      user.healthHaver.ApplyHealing(this.healingAmount);
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", this.gameObject);
      if (!((Object) this.healVFX != (Object) null))
        return;
      user.PlayEffectOnActor(this.healVFX, Vector3.zero);
    }

    public void DoHealOnDeath(PlayerController user) => this.DoEffect(user);

    protected override void OnDestroy() => base.OnDestroy();
  }

