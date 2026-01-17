// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StopLocationServiceUpdates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Device)]
[Tooltip("Stops location service updates. This could be useful for saving battery life.")]
public class StopLocationServiceUpdates : FsmStateAction
{
  public override void Reset()
  {
  }

  public override void OnEnter() => this.Finish();
}
