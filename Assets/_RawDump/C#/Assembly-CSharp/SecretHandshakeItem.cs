// Decompiled with JetBrains decompiler
// Type: SecretHandshakeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class SecretHandshakeItem : PassiveItem
{
  public static int NumActive;

  private void Awake()
  {
  }

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    ++SecretHandshakeItem.NumActive;
    base.Pickup(player);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    --SecretHandshakeItem.NumActive;
    debrisObject.GetComponent<SecretHandshakeItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    if (this.m_pickedUp)
      --SecretHandshakeItem.NumActive;
    base.OnDestroy();
  }
}
