// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetTimeInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets various useful Time measurements.")]
[ActionCategory(ActionCategory.Time)]
public class GetTimeInfo : FsmStateAction
{
  public GetTimeInfo.TimeInfo getInfo;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmFloat storeValue;
  public bool everyFrame;

  public override void Reset()
  {
    this.getInfo = GetTimeInfo.TimeInfo.TimeSinceLevelLoad;
    this.storeValue = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetTimeInfo();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetTimeInfo();

  private void DoGetTimeInfo()
  {
    switch (this.getInfo)
    {
      case GetTimeInfo.TimeInfo.DeltaTime:
        this.storeValue.Value = Time.deltaTime;
        break;
      case GetTimeInfo.TimeInfo.TimeScale:
        this.storeValue.Value = Time.timeScale;
        break;
      case GetTimeInfo.TimeInfo.SmoothDeltaTime:
        this.storeValue.Value = Time.smoothDeltaTime;
        break;
      case GetTimeInfo.TimeInfo.TimeInCurrentState:
        this.storeValue.Value = this.State.StateTime;
        break;
      case GetTimeInfo.TimeInfo.TimeSinceStartup:
        this.storeValue.Value = Time.time;
        break;
      case GetTimeInfo.TimeInfo.TimeSinceLevelLoad:
        this.storeValue.Value = Time.timeSinceLevelLoad;
        break;
      case GetTimeInfo.TimeInfo.RealTimeSinceStartup:
        this.storeValue.Value = FsmTime.RealtimeSinceStartup;
        break;
      case GetTimeInfo.TimeInfo.RealTimeInCurrentState:
        this.storeValue.Value = FsmTime.RealtimeSinceStartup - this.State.RealStartTime;
        break;
      default:
        this.storeValue.Value = 0.0f;
        break;
    }
  }

  public enum TimeInfo
  {
    DeltaTime,
    TimeScale,
    SmoothDeltaTime,
    TimeInCurrentState,
    TimeSinceStartup,
    TimeSinceLevelLoad,
    RealTimeSinceStartup,
    RealTimeInCurrentState,
  }
}
