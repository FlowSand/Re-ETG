// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ChangeSaveStat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sends Events based on the data in a player save.")]
[ActionCategory(".Brave")]
public class ChangeSaveStat : FsmStateAction
{
  public TrackedStats stat;
  public FsmFloat statChange;

  public override void Reset()
  {
    this.stat = TrackedStats.BULLETS_FIRED;
    this.statChange = (FsmFloat) 0.0f;
  }

  public override string ErrorCheck() => string.Empty;

  public override void OnEnter()
  {
    GameStatsManager.Instance.RegisterStatChange(this.stat, this.statChange.Value);
    this.Finish();
  }
}
