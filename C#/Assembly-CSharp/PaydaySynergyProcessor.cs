// Decompiled with JetBrains decompiler
// Type: PaydaySynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class PaydaySynergyProcessor : MonoBehaviour
{
  [PickupIdentifier]
  public int ItemID01;
  [PickupIdentifier]
  public int ItemID02;
  [PickupIdentifier]
  public int ItemID03;
  private PlayerController m_player;

  [DebuggerHidden]
  public IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new PaydaySynergyProcessor.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private List<IPaydayItem> GetExtantPaydayItems()
  {
    List<IPaydayItem> extantPaydayItems = new List<IPaydayItem>();
    for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
      if ((bool) (Object) allPlayer && !allPlayer.IsGhost)
      {
        for (int index2 = 0; index2 < allPlayer.activeItems.Count; ++index2)
        {
          if (allPlayer.activeItems[index2] is IPaydayItem)
            extantPaydayItems.Add(allPlayer.activeItems[index2] as IPaydayItem);
        }
        for (int index3 = 0; index3 < allPlayer.passiveItems.Count; ++index3)
        {
          if (allPlayer.passiveItems[index3] is IPaydayItem)
            extantPaydayItems.Add(allPlayer.passiveItems[index3] as IPaydayItem);
        }
      }
    }
    return extantPaydayItems;
  }

  public void Initialize(PlayerController ownerPlayer)
  {
    if ((Object) ownerPlayer == (Object) null)
      return;
    this.m_player = ownerPlayer;
    CompanionSynergyProcessor[] components = this.GetComponents<CompanionSynergyProcessor>();
    List<string> input = new List<string>();
    for (int index = 0; index < components.Length; ++index)
    {
      input.Add(components[index].CompanionGuid);
      components[index].ManuallyAssignedPlayer = this.m_player;
    }
    List<IPaydayItem> extantPaydayItems = this.GetExtantPaydayItems();
    bool flag = false;
    IPaydayItem paydayItem = (IPaydayItem) null;
    for (int index = 0; index < extantPaydayItems.Count; ++index)
    {
      if (extantPaydayItems[index].HasCachedData())
      {
        flag = true;
        paydayItem = extantPaydayItems[index];
        break;
      }
    }
    if (flag)
    {
      input.Clear();
      input.Add(paydayItem.GetID(0));
      input.Add(paydayItem.GetID(1));
      input.Add(paydayItem.GetID(2));
      for (int index = 0; index < components.Length; ++index)
        components[index].CompanionGuid = input[index];
    }
    else
    {
      List<string> stringList = input.Shuffle<string>();
      for (int index = 0; index < components.Length; ++index)
        components[index].CompanionGuid = stringList[index];
      for (int index1 = 0; index1 < components.Length; ++index1)
      {
        for (int index2 = 0; index2 < extantPaydayItems.Count; ++index2)
          extantPaydayItems[index2].StoreData(components[0].CompanionGuid, components[1].CompanionGuid, components[2].CompanionGuid);
      }
    }
  }

  public void Update()
  {
    int num = 0;
    bool flag = false;
    if (!(bool) (Object) this.m_player)
      this.Initialize(this.transform.parent.GetComponent<PlayerController>());
    for (int index = 0; index < this.m_player.passiveItems.Count; ++index)
    {
      if (this.m_player.passiveItems[index] is BankMaskItem)
        flag = true;
      if (this.m_player.passiveItems[index].PickupObjectId == this.ItemID01)
        ++num;
      if (this.m_player.passiveItems[index].PickupObjectId == this.ItemID02)
        ++num;
      if (this.m_player.passiveItems[index].PickupObjectId == this.ItemID03)
        ++num;
    }
    for (int index = 0; index < this.m_player.activeItems.Count; ++index)
    {
      if (this.m_player.activeItems[index].PickupObjectId == this.ItemID01)
        ++num;
      if (this.m_player.activeItems[index].PickupObjectId == this.ItemID02)
        ++num;
      if (this.m_player.activeItems[index].PickupObjectId == this.ItemID03)
        ++num;
    }
    if (!flag)
      num = 0;
    this.m_player.CustomEventSynergies.Remove(CustomSynergyType.PAYDAY_ONEITEM);
    this.m_player.CustomEventSynergies.Remove(CustomSynergyType.PAYDAY_TWOITEM);
    this.m_player.CustomEventSynergies.Remove(CustomSynergyType.PAYDAY_THREEITEM);
    if (num <= 0)
      return;
    if (num == 1)
      this.m_player.CustomEventSynergies.Add(CustomSynergyType.PAYDAY_ONEITEM);
    else if (num == 2)
    {
      this.m_player.CustomEventSynergies.Add(CustomSynergyType.PAYDAY_ONEITEM);
      this.m_player.CustomEventSynergies.Add(CustomSynergyType.PAYDAY_TWOITEM);
    }
    else
    {
      this.m_player.CustomEventSynergies.Add(CustomSynergyType.PAYDAY_ONEITEM);
      this.m_player.CustomEventSynergies.Add(CustomSynergyType.PAYDAY_TWOITEM);
      this.m_player.CustomEventSynergies.Add(CustomSynergyType.PAYDAY_THREEITEM);
    }
  }
}
