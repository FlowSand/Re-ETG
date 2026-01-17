// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DisconnectState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Removes all transitions to this state.")]
[ActionCategory(".Brave")]
public class DisconnectState : FsmStateAction
{
  public override void OnEnter()
  {
    BravePlayMakerUtility.DisconnectState(this.State);
    this.Finish();
  }
}
