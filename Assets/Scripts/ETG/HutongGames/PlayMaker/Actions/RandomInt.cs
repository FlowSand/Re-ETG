// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RandomInt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Math)]
  [HutongGames.PlayMaker.Tooltip("Sets an Integer Variable to a random value between Min/Max.")]
  public class RandomInt : FsmStateAction
  {
    [RequiredField]
    public FsmInt min;
    [RequiredField]
    public FsmInt max;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt storeResult;
    [HutongGames.PlayMaker.Tooltip("Should the Max value be included in the possible results?")]
    public bool inclusiveMax;

    public override void Reset()
    {
      this.min = (FsmInt) 0;
      this.max = (FsmInt) 100;
      this.storeResult = (FsmInt) null;
      this.inclusiveMax = false;
    }

    public override void OnEnter()
    {
      this.storeResult.Value = !this.inclusiveMax ? Random.Range(this.min.Value, this.max.Value) : Random.Range(this.min.Value, this.max.Value + 1);
      this.Finish();
    }
  }
}
