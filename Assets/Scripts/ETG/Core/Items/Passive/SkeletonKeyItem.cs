// Decompiled with JetBrains decompiler
// Type: SkeletonKeyItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Items.Passive
{
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

}
