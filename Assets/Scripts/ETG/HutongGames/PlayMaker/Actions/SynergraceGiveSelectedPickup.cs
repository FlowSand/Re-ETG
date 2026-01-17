// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SynergraceGiveSelectedPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Completes a synergy. Requires SynergraceTestCompletionPossible.")]
  [ActionCategory(".Brave")]
  public class SynergraceGiveSelectedPickup : BraveFsmStateAction
  {
    public override void OnEnter()
    {
      base.OnEnter();
      TalkDoerLite component1 = this.Owner.GetComponent<TalkDoerLite>();
      SynergraceTestCompletionPossible actionOfType = this.FindActionOfType<SynergraceTestCompletionPossible>();
      if ((bool) (Object) component1 && (bool) (Object) component1.TalkingPlayer && actionOfType != null && (bool) (Object) actionOfType.SelectedPickupGameObject)
      {
        Chest chest = Chest.Spawn(GameManager.Instance.RewardManager.Synergy_Chest, component1.transform.position.IntXY(VectorConversions.Floor) + new IntVector2(1, -5));
        if ((bool) (Object) chest)
        {
          chest.IsLocked = false;
          PickupObject component2 = actionOfType.SelectedPickupGameObject.GetComponent<PickupObject>();
          if ((bool) (Object) component2)
          {
            chest.forceContentIds = new List<int>();
            chest.forceContentIds.Add(component2.PickupObjectId);
          }
        }
        else
          LootEngine.TryGivePrefabToPlayer(actionOfType.SelectedPickupGameObject, component1.TalkingPlayer);
        actionOfType.SelectedPickupGameObject = (GameObject) null;
        component1.TalkingPlayer.HandleItemPurchased((ShopItemController) null);
      }
      this.Finish();
    }
  }
}
