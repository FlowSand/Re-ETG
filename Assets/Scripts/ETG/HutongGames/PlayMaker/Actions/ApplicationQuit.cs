// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ApplicationQuit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Quits the player application.")]
  [ActionCategory(ActionCategory.Application)]
  public class ApplicationQuit : FsmStateAction
  {
    public override void Reset()
    {
    }

    public override void OnEnter()
    {
      Application.Quit();
      this.Finish();
    }
  }
}
