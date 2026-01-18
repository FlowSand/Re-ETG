#nullable disable

public class SkeletonKeyItem : PassiveItem
  {
    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].carriedConsumables.InfiniteKeys = true;
      base.Pickup(player);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<SkeletonKeyItem>().m_pickedUpThisRun = true;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].carriedConsumables.InfiniteKeys = false;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].carriedConsumables.InfiniteKeys = false;
      base.OnDestroy();
    }
  }

