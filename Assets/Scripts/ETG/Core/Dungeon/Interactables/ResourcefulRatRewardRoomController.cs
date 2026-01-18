using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

public class ResourcefulRatRewardRoomController : DungeonPlaceableBehaviour
  {
    [PickupIdentifier]
    public int[] PedestalA1Items;
    [PickupIdentifier]
    public int[] PedestalA2Items;
    [PickupIdentifier]
    public int[] PedestalA3Items;
    [PickupIdentifier]
    public int[] PedestalA4Items;
    [PickupIdentifier]
    public int[] PedestalB1Items;
    [PickupIdentifier]
    public int[] PedestalB2Items;
    [PickupIdentifier]
    public int[] PedestalB3Items;
    [PickupIdentifier]
    public int[] PedestalB4Items;
    [PickupIdentifier]
    public int[] RatChestItems;
    private RewardPedestal[] m_pedestals;
    private Chest[] m_ratChests;

    public void Start()
    {
      RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
      this.m_pedestals = absoluteRoom.GetComponentsInRoom<RewardPedestal>().ttOrderByDescending<RewardPedestal, float>((Func<RewardPedestal, float>) (a => a.transform.position.x * 10000f + a.transform.position.y)).ToArray();
      this.m_pedestals[0].SpecificItemId = this.PedestalA1Items[UnityEngine.Random.Range(0, this.PedestalA1Items.Length)];
      this.m_pedestals[1].SpecificItemId = this.PedestalA2Items[UnityEngine.Random.Range(0, this.PedestalA2Items.Length)];
      this.m_pedestals[2].SpecificItemId = this.PedestalA3Items[UnityEngine.Random.Range(0, this.PedestalA3Items.Length)];
      this.m_pedestals[3].SpecificItemId = this.PedestalA4Items[UnityEngine.Random.Range(0, this.PedestalA4Items.Length)];
      this.m_pedestals[4].SpecificItemId = this.PedestalB1Items[UnityEngine.Random.Range(0, this.PedestalB1Items.Length)];
      this.m_pedestals[5].SpecificItemId = this.PedestalB2Items[UnityEngine.Random.Range(0, this.PedestalB2Items.Length)];
      this.m_pedestals[6].SpecificItemId = this.PedestalB3Items[UnityEngine.Random.Range(0, this.PedestalB3Items.Length)];
      this.m_pedestals[7].SpecificItemId = this.PedestalB4Items[UnityEngine.Random.Range(0, this.PedestalB4Items.Length)];
      for (int index = 0; index < this.m_pedestals.Length; ++index)
        this.m_pedestals[index].ForceConfiguration();
      List<Chest> componentsInRoom = absoluteRoom.GetComponentsInRoom<Chest>();
      this.m_ratChests = new Chest[4];
      int index1 = 0;
      for (int index2 = 0; index2 < componentsInRoom.Count; ++index2)
      {
        if (componentsInRoom[index2].ChestIdentifier == Chest.SpecialChestIdentifier.RAT)
        {
          this.m_ratChests[index1] = componentsInRoom[index2];
          ++index1;
          if (index1 >= this.m_ratChests.Length)
            break;
        }
      }
      List<int> intList = Enumerable.Range(0, this.RatChestItems.Length).ToList<int>().Shuffle<int>();
      for (int index3 = 0; index3 < this.m_ratChests.Length; ++index3)
      {
        this.m_ratChests[index3].forceContentIds = new List<int>();
        this.m_ratChests[index3].forceContentIds.Add(this.RatChestItems[intList[index3]]);
      }
    }
  }

