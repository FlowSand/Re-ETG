// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatCompare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends Events based on the comparison of 2 Floats.")]
[ActionCategory(ActionCategory.Logic)]
public class FloatCompare : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The first float variable.")]
  [RequiredField]
  public FsmFloat float1;
  [HutongGames.PlayMaker.Tooltip("The second float variable.")]
  [RequiredField]
  public FsmFloat float2;
  [HutongGames.PlayMaker.Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
  [RequiredField]
  public FsmFloat tolerance;
  [HutongGames.PlayMaker.Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
  public FsmEvent equal;
  [HutongGames.PlayMaker.Tooltip("Event sent if Float 1 is less than Float 2")]
  public FsmEvent lessThan;
  [HutongGames.PlayMaker.Tooltip("Event sent if Float 1 is greater than Float 2")]
  public FsmEvent greaterThan;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.float1 = (FsmFloat) 0.0f;
    this.float2 = (FsmFloat) 0.0f;
    this.tolerance = (FsmFloat) 0.0f;
    this.equal = (FsmEvent) null;
    this.lessThan = (FsmEvent) null;
    this.greaterThan = (FsmEvent) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoCompare();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoCompare();

  private void DoCompare()
  {
    if ((double) Mathf.Abs(this.float1.Value - this.float2.Value) <= (double) this.tolerance.Value)
      this.Fsm.Event(this.equal);
    else if ((double) this.float1.Value < (double) this.float2.Value)
    {
      this.Fsm.Event(this.lessThan);
    }
    else
    {
      if ((double) this.float1.Value <= (double) this.float2.Value)
        return;
      this.Fsm.Event(this.greaterThan);
    }
  }

  public override string ErrorCheck()
  {
    return FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan) ? "Action sends no events!" : string.Empty;
  }
}
