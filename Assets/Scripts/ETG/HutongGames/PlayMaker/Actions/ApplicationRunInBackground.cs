// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ApplicationRunInBackground
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets if the Application should play in the background. Useful for servers or testing network games on one machine.")]
  [ActionCategory(ActionCategory.Application)]
  public class ApplicationRunInBackground : FsmStateAction
  {
    public FsmBool runInBackground;

    public override void Reset() => this.runInBackground = (FsmBool) true;

    public override void OnEnter()
    {
      Application.runInBackground = this.runInBackground.Value;
      this.Finish();
    }
  }
}
