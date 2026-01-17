// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IntCompare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends Events based on the comparison of 2 Integers.")]
  [ActionCategory(ActionCategory.Logic)]
  public class IntCompare : FsmStateAction
  {
    [RequiredField]
    public FsmInt integer1;
    [RequiredField]
    public FsmInt integer2;
    [Tooltip("Event sent if Int 1 equals Int 2")]
    public FsmEvent equal;
    [Tooltip("Event sent if Int 1 is less than Int 2")]
    public FsmEvent lessThan;
    [Tooltip("Event sent if Int 1 is greater than Int 2")]
    public FsmEvent greaterThan;
    public bool everyFrame;

    public override void Reset()
    {
      this.integer1 = (FsmInt) 0;
      this.integer2 = (FsmInt) 0;
      this.equal = (FsmEvent) null;
      this.lessThan = (FsmEvent) null;
      this.greaterThan = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoIntCompare();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoIntCompare();

    private void DoIntCompare()
    {
      if (this.integer1.Value == this.integer2.Value)
        this.Fsm.Event(this.equal);
      else if (this.integer1.Value < this.integer2.Value)
      {
        this.Fsm.Event(this.lessThan);
      }
      else
      {
        if (this.integer1.Value <= this.integer2.Value)
          return;
        this.Fsm.Event(this.greaterThan);
      }
    }

    public override string ErrorCheck()
    {
      return FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan) ? "Action sends no events!" : string.Empty;
    }
  }
}
