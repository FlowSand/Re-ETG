// Decompiled with JetBrains decompiler
// Type: AstralSlugItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class AstralSlugItem : PassiveItem
{
  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    debrisObject.GetComponent<AstralSlugItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
