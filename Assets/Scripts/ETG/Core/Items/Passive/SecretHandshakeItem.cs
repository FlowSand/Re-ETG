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

