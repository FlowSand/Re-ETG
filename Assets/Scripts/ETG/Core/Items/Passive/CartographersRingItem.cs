#nullable disable

public class CartographersRingItem : PassiveItem
  {
    public float revealChanceOnLoad = 0.5f;
    public bool revealSecretRooms;
    public bool executeOnPickup;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      bool flag = false;
      if (this.executeOnPickup && !this.m_pickedUpThisRun)
        flag = true;
      base.Pickup(player);
      if (flag)
        this.PossiblyRevealMap();
      GameManager.Instance.OnNewLevelFullyLoaded += new System.Action(this.PossiblyRevealMap);
    }

    public void PossiblyRevealMap()
    {
      if ((double) UnityEngine.Random.value >= (double) this.revealChanceOnLoad || !((UnityEngine.Object) Minimap.Instance != (UnityEngine.Object) null))
        return;
      Minimap.Instance.RevealAllRooms(this.revealSecretRooms);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<CartographersRingItem>().m_pickedUpThisRun = true;
      GameManager.Instance.OnNewLevelFullyLoaded -= new System.Action(this.PossiblyRevealMap);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if (this.m_pickedUp && GameManager.HasInstance)
        GameManager.Instance.OnNewLevelFullyLoaded -= new System.Action(this.PossiblyRevealMap);
      base.OnDestroy();
    }
  }

