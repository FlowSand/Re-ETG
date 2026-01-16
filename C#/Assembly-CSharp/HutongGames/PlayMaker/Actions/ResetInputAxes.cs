// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ResetInputAxes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Resets all Input. After ResetInputAxes all axes return to 0 and all buttons return to 0 for one frame")]
[ActionCategory(ActionCategory.Input)]
public class ResetInputAxes : FsmStateAction
{
  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    Input.ResetInputAxes();
    this.Finish();
  }
}
