// Decompiled with JetBrains decompiler
// Type: MagazineRackItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
