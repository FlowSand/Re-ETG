// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetScreenHeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the Height of the Screen in pixels.")]
[ActionCategory(ActionCategory.Application)]
public class GetScreenHeight : FsmStateAction
{
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmFloat storeScreenHeight;

  public override void Reset() => this.storeScreenHeight = (FsmFloat) null;

  public override void OnEnter()
  {
    this.storeScreenHeight.Value = (float) Screen.height;
    this.Finish();
  }
}
