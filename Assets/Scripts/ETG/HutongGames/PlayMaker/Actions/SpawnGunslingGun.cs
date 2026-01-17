// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SpawnGunslingGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  public class SpawnGunslingGun : BraveFsmStateAction
  {
    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      PlayerController player1 = !(bool) (Object) component.TalkingPlayer ? GameManager.Instance.PrimaryPlayer : component.TalkingPlayer;
      SelectGunslingGun actionOfType1 = this.FindActionOfType<SelectGunslingGun>();
      CheckGunslingChallengeComplete actionOfType2 = this.FindActionOfType<CheckGunslingChallengeComplete>();
      if (actionOfType1 != null)
      {
        Gun player2 = LootEngine.TryGiveGunToPlayer(actionOfType1.SelectedObject, player1);
        if ((bool) (Object) player2)
        {
          player2.CanBeDropped = false;
          player2.CanBeSold = false;
          player2.IsMinusOneGun = true;
          if (actionOfType2 != null)
          {
            actionOfType2.GunToUse = player2;
            actionOfType2.GunToUsePrefab = actionOfType1.SelectedObject.GetComponent<Gun>();
          }
        }
      }
      this.Finish();
    }
  }
}
