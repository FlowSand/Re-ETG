using UnityEngine;

#nullable disable

public class ArmoredGunModifier : MonoBehaviour
  {
    [PickupIdentifier]
    public int ArmoredId = -1;
    [PickupIdentifier]
    public int UnarmoredId = -1;
    [CheckAnimation(null)]
    public string ArmorUpAnimation;
    [CheckAnimation(null)]
    public string ArmorLostAnimation;
    private Gun m_gun;
    private bool m_armored = true;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      if (this.ArmoredId < 0)
        this.ArmoredId = PickupObjectDatabase.GetById(this.UnarmoredId).GetComponent<ArmoredGunModifier>().ArmoredId;
      if (this.UnarmoredId >= 0)
        return;
      this.UnarmoredId = PickupObjectDatabase.GetById(this.ArmoredId).GetComponent<ArmoredGunModifier>().UnarmoredId;
    }

    private void Update()
    {
      if ((bool) (Object) this.m_gun && !(bool) (Object) this.m_gun.CurrentOwner)
      {
        PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
        if (!(bool) (Object) bestActivePlayer)
          return;
        if ((bool) (Object) bestActivePlayer.healthHaver && (double) bestActivePlayer.healthHaver.Armor > 0.0)
          this.m_gun.sprite.SetSprite((PickupObjectDatabase.GetById(this.ArmoredId) as Gun).sprite.spriteId);
        else
          this.m_gun.sprite.SetSprite((PickupObjectDatabase.GetById(this.UnarmoredId) as Gun).sprite.spriteId);
      }
      else
      {
        if (!(bool) (Object) this.m_gun || !(bool) (Object) this.m_gun.CurrentOwner || !(bool) (Object) this.m_gun.CurrentOwner.healthHaver)
          return;
        float num = this.m_gun.CurrentOwner.healthHaver.Armor;
        if (this.m_gun.OwnerHasSynergy(CustomSynergyType.NANOARMOR))
          num = 20f;
        if (this.m_armored && (double) num <= 0.0)
        {
          this.BecomeUnarmored();
        }
        else
        {
          if (this.m_armored || (double) num <= 0.0)
            return;
          this.BecomeArmored();
        }
      }
    }

    private void BecomeArmored()
    {
      if (this.m_armored)
        return;
      this.m_armored = true;
      this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.ArmoredId) as Gun);
      this.m_gun.spriteAnimator.Play(this.ArmorUpAnimation);
    }

    private void BecomeUnarmored()
    {
      if (!this.m_armored)
        return;
      this.m_armored = false;
      this.m_gun.TransformToTargetGun(PickupObjectDatabase.GetById(this.UnarmoredId) as Gun);
      this.m_gun.spriteAnimator.Play(this.ArmorLostAnimation);
    }
  }

