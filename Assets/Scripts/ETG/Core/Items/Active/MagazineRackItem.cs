using UnityEngine;

#nullable disable

public class MagazineRackItem : PlayerItem
  {
    public GameObject MagazineRackPrefab;
    private GameObject m_instanceRack;

    public override bool CanBeUsed(PlayerController user)
    {
      return !(bool) (Object) this.m_instanceRack && base.CanBeUsed(user);
    }

    protected override void DoEffect(PlayerController user)
    {
      this.m_instanceRack = Object.Instantiate<GameObject>(this.MagazineRackPrefab, user.CenterPosition.ToVector3ZisY(), Quaternion.identity, (Transform) null);
    }
  }

