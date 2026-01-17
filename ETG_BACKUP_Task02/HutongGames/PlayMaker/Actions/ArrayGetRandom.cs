// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayGetRandom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get a Random item from an Array.")]
[ActionCategory(ActionCategory.Array)]
public class ArrayGetRandom : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The Array to use.")]
  [UIHint(UIHint.Variable)]
  public FsmArray array;
  [HutongGames.PlayMaker.Tooltip("Store the value in a variable.")]
  [MatchElementType("array")]
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmVar storeValue;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.array = (FsmArray) null;
    this.storeValue = (FsmVar) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetRandomValue();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetRandomValue();

  private void DoGetRandomValue()
  {
    if (this.storeValue.IsNone)
      return;
    this.storeValue.SetValue(this.array.Get(Random.Range(0, this.array.Length)));
  }
}
