// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.IntOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Math)]
  [HutongGames.PlayMaker.Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
  public class IntOperator : FsmStateAction
  {
    [RequiredField]
    public FsmInt integer1;
    [RequiredField]
    public FsmInt integer2;
    public IntOperator.Operation operation;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmInt storeResult;
    public bool everyFrame;

    public override void Reset()
    {
      this.integer1 = (FsmInt) null;
      this.integer2 = (FsmInt) null;
      this.operation = IntOperator.Operation.Add;
      this.storeResult = (FsmInt) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoIntOperator();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoIntOperator();

    private void DoIntOperator()
    {
      int a = this.integer1.Value;
      int b = this.integer2.Value;
      switch (this.operation)
      {
        case IntOperator.Operation.Add:
          this.storeResult.Value = a + b;
          break;
        case IntOperator.Operation.Subtract:
          this.storeResult.Value = a - b;
          break;
        case IntOperator.Operation.Multiply:
          this.storeResult.Value = a * b;
          break;
        case IntOperator.Operation.Divide:
          this.storeResult.Value = a / b;
          break;
        case IntOperator.Operation.Min:
          this.storeResult.Value = Mathf.Min(a, b);
          break;
        case IntOperator.Operation.Max:
          this.storeResult.Value = Mathf.Max(a, b);
          break;
      }
    }

    public enum Operation
    {
      Add,
      Subtract,
      Multiply,
      Divide,
      Min,
      Max,
    }
  }
}
