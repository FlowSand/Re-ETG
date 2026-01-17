// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.LaunchTimedEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".Brave")]
public class LaunchTimedEvent : FsmStateAction
{
  public GungeonFlags targetFlag;
  public float AllotedTime = 60f;

  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    GameManager.Instance.LaunchTimedEvent(this.AllotedTime, (Action<bool>) (a => GameStatsManager.Instance.SetFlag(this.targetFlag, a)));
    this.Finish();
  }
}
