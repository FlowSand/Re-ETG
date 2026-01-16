// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RandomBool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Math)]
[HutongGames.PlayMaker.Tooltip("Sets a Bool Variable to True or False randomly.")]
public class RandomBool : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;

  public override void Reset() => this.storeResult = (FsmBool) null;

  public override void OnEnter()
  {
    this.storeResult.Value = Random.Range(0, 100) < 50;
    this.Finish();
  }
}
