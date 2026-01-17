// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TakeConsumable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [Tooltip("Takes a consumable from the player (heart, key, currency, etc.).")]
  public class TakeConsumable : FsmStateAction
  {
    [Tooltip("Type of consumable to take.")]
    public BravePlayMakerUtility.ConsumableType consumableType;
    [Tooltip("Amount of the consumable to take.")]
    public FsmFloat amount;
    [Tooltip("The event to send if the player pays.")]
    public FsmEvent success;
    [Tooltip("The event to send if the player does not have enough of the consumable.")]
    public FsmEvent failure;

    public override void Reset()
    {
      this.consumableType = BravePlayMakerUtility.ConsumableType.Currency;
      this.amount = (FsmFloat) 0.0f;
      this.failure = (FsmEvent) null;
    }

    public override string ErrorCheck()
    {
      string empty = string.Empty;
      if (!this.amount.UsesVariable && (double) this.amount.Value <= 0.0)
        empty += "Need to take at least some number of consumable.\n";
      return empty;
    }

    public override void OnEnter()
    {
      PlayerController talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
      float consumableValue = BravePlayMakerUtility.GetConsumableValue(talkingPlayer, this.consumableType);
      if ((double) consumableValue >= (double) this.amount.Value)
      {
        BravePlayMakerUtility.SetConsumableValue(talkingPlayer, this.consumableType, consumableValue - this.amount.Value);
        this.Fsm.Event(this.success);
      }
      else
        this.Fsm.Event(this.failure);
      this.Finish();
    }
  }
}
