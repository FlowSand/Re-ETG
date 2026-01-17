// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetScreenWidth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Application)]
  [HutongGames.PlayMaker.Tooltip("Gets the Width of the Screen in pixels.")]
  public class GetScreenWidth : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeScreenWidth;

    public override void Reset() => this.storeScreenWidth = (FsmFloat) null;

    public override void OnEnter()
    {
      this.storeScreenWidth.Value = (float) Screen.width;
      this.Finish();
    }
  }
}
