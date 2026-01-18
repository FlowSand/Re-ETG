using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [HutongGames.PlayMaker.Tooltip("Gives a consumable to the player (heart, key, currency, etc.).")]
  public class GiveConsumable : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Type of consumable to give.")]
    public BravePlayMakerUtility.ConsumableType consumableType;
    [HutongGames.PlayMaker.Tooltip("Amount of the consumable to give.")]
    public FsmFloat amount;

    public override void Reset()
    {
      this.consumableType = BravePlayMakerUtility.ConsumableType.Currency;
      this.amount = (FsmFloat) 0.0f;
    }

    public override string ErrorCheck()
    {
      string empty = string.Empty;
      if (!this.amount.UsesVariable && (double) this.amount.Value <= 0.0)
        empty += "Need to give at least some number of consumable.\n";
      return empty;
    }

    public override void OnEnter()
    {
      PlayerController talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
      float consumableValue = BravePlayMakerUtility.GetConsumableValue(talkingPlayer, this.consumableType);
      BravePlayMakerUtility.SetConsumableValue(talkingPlayer, this.consumableType, consumableValue + this.amount.Value);
      if (this.consumableType == BravePlayMakerUtility.ConsumableType.Hearts && (double) this.amount.Value > 0.0)
      {
        GameObject effect = BraveResources.Load<GameObject>("Global VFX/VFX_Healing_Sparkles_001");
        if ((Object) effect != (Object) null)
          talkingPlayer.PlayEffectOnActor(effect, Vector3.zero);
      }
      this.Finish();
    }
  }
}
