// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestSaveStat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".Brave")]
[Tooltip("Sends Events based on the data in a player save.")]
public class TestSaveStat : FsmStateAction
{
  [Tooltip("Type of save data to lookup.")]
  public TestSaveStat.SaveType saveType;
  [Tooltip("Stat to check")]
  public TrackedStats stat;
  public TestSaveStat.StatGroup statGroup;
  [Tooltip("Stat must be greather than or equal to this value to pass the test.")]
  public FsmFloat minValue;
  [Tooltip("The ID of the encounterable object.")]
  public FsmString encounterId;
  [Tooltip("The ID of the encounterable object.")]
  public FsmString encounterGuid;
  [Tooltip("The event to send if the test passes.")]
  public FsmEvent Event;

  public override void Reset()
  {
    this.saveType = TestSaveStat.SaveType.Stat;
    this.stat = TrackedStats.BULLETS_FIRED;
    this.statGroup = TestSaveStat.StatGroup.Global;
    this.minValue = (FsmFloat) 0.0f;
    this.encounterGuid = (FsmString) string.Empty;
    this.Event = (FsmEvent) null;
  }

  public override string ErrorCheck()
  {
    string empty = string.Empty;
    if (this.saveType == TestSaveStat.SaveType.Stat)
    {
      if ((double) this.minValue.Value <= 0.0)
        empty += "Min Value must be greater than 0.\n";
    }
    else if (this.saveType == TestSaveStat.SaveType.EncounteredTrackable)
    {
      if (EncounterDatabase.GetEntry(this.encounterGuid.Value) == null)
        empty += "Invalid encounter ID.\n";
    }
    else if (this.saveType == TestSaveStat.SaveType.EncounteredRoom && string.IsNullOrEmpty(this.encounterId.Value))
      empty += "Invalid room ID.\n";
    return empty;
  }

  public override void OnEnter()
  {
    this.DoCheck();
    this.Finish();
  }

  private void DoCheck()
  {
    float num = -1f;
    if (this.saveType == TestSaveStat.SaveType.Stat)
    {
      if (this.statGroup == TestSaveStat.StatGroup.Global)
        num = GameStatsManager.Instance.GetPlayerStatValue(this.stat);
      else if (this.statGroup == TestSaveStat.StatGroup.Character)
        num = GameStatsManager.Instance.GetCharacterStatValue(this.stat);
      else if (this.statGroup == TestSaveStat.StatGroup.Session)
        num = GameStatsManager.Instance.GetSessionStatValue(this.stat);
    }
    else if (this.saveType == TestSaveStat.SaveType.EncounteredTrackable)
      num = (float) GameStatsManager.Instance.QueryEncounterable(this.encounterGuid.Value);
    else if (this.saveType == TestSaveStat.SaveType.EncounteredRoom)
      num = (float) GameStatsManager.Instance.QueryRoomEncountered(this.encounterId.Value);
    if ((double) num < (double) this.minValue.Value)
      return;
    this.Fsm.Event(this.Event);
  }

  public enum SaveType
  {
    Stat,
    EncounteredTrackable,
    EncounteredRoom,
  }

  public enum StatGroup
  {
    Global,
    Character,
    Session,
  }
}
