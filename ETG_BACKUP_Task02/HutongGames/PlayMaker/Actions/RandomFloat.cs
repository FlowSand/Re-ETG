// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RandomFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Math)]
[HutongGames.PlayMaker.Tooltip("Sets a Float Variable to a random value between Min/Max.")]
public class RandomFloat : FsmStateAction
{
  [RequiredField]
  public FsmFloat min;
  [RequiredField]
  public FsmFloat max;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat storeResult;

  public override void Reset()
  {
    this.min = (FsmFloat) 0.0f;
    this.max = (FsmFloat) 1f;
    this.storeResult = (FsmFloat) null;
  }

  public override void OnEnter()
  {
    this.storeResult.Value = Random.Range(this.min.Value, this.max.Value);
    this.Finish();
  }
}
