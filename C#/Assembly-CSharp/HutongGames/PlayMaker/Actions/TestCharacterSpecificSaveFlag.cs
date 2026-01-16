// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestCharacterSpecificSaveFlag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".Brave")]
[Tooltip("Sends Events based on the value of a player save flag.")]
public class TestCharacterSpecificSaveFlag : FsmStateAction
{
  public TestCharacterSpecificSaveFlag.SuccessType successType;
  public CharacterSpecificGungeonFlags[] flagValues;
  public FsmBool[] values;
  [Tooltip("The event to send if the proceeding tests all pass.")]
  public FsmEvent Event;
  [Tooltip("The name of the mode to set 'currentMode' to if the proceeding tests all pass.")]
  public FsmString mode;
  public FsmBool everyFrame;
  private bool m_success;

  public bool Success => this.m_success;

  public override void Reset()
  {
    this.successType = TestCharacterSpecificSaveFlag.SuccessType.SetMode;
    this.flagValues = new CharacterSpecificGungeonFlags[0];
    this.values = new FsmBool[0];
    this.Event = (FsmEvent) null;
    this.mode = (FsmString) string.Empty;
  }

  public override string ErrorCheck()
  {
    string str1 = string.Empty;
    for (int index = 0; index < this.flagValues.Length; ++index)
    {
      if (this.flagValues[index] == CharacterSpecificGungeonFlags.NONE)
        str1 += "Flag Value is NONE. This is a mistake.";
    }
    if (this.successType == TestCharacterSpecificSaveFlag.SuccessType.SetMode)
    {
      string str2 = str1 + BravePlayMakerUtility.CheckCurrentModeVariable(this.Fsm);
      if (!this.mode.Value.StartsWith("mode"))
        str2 += "Let's be civil and start all mode names with \"mode\", okay?\n";
      str1 = str2 + BravePlayMakerUtility.CheckEventExists(this.Fsm, this.mode.Value) + BravePlayMakerUtility.CheckGlobalTransitionExists(this.Fsm, this.mode.Value);
    }
    return str1;
  }

  public override void OnEnter()
  {
    if (this.ShouldSkip())
    {
      this.m_success = true;
      this.Finish();
    }
    else
    {
      this.DoCheck();
      if (this.everyFrame.Value)
        return;
      this.Finish();
    }
  }

  public override void OnUpdate()
  {
    if (this.ShouldSkip())
    {
      this.m_success = true;
      this.Finish();
    }
    else
      this.DoCheck();
  }

  private bool ShouldSkip()
  {
    for (int index = 0; index < this.State.Actions.Length && this.State.Actions[index] != this; ++index)
    {
      if (this.State.Actions[index] is TestSaveFlag && (this.State.Actions[index] as TestSaveFlag).Success || this.State.Actions[index] is TestCharacterSpecificSaveFlag && (this.State.Actions[index] as TestCharacterSpecificSaveFlag).Success)
        return true;
    }
    return false;
  }

  private void DoCheck()
  {
    this.m_success = true;
    for (int index = 0; index < this.flagValues.Length; ++index)
    {
      if (GameStatsManager.Instance.GetCharacterSpecificFlag(this.flagValues[index]) != this.values[index].Value)
      {
        this.m_success = false;
        break;
      }
    }
    if (!this.m_success)
      return;
    if (this.successType == TestCharacterSpecificSaveFlag.SuccessType.SendEvent)
      this.Fsm.Event(this.Event);
    else if (this.successType == TestCharacterSpecificSaveFlag.SuccessType.SetMode)
      this.Fsm.Variables.GetFsmString("currentMode").Value = this.mode.Value;
    this.Finish();
  }

  public enum SuccessType
  {
    SetMode,
    SendEvent,
  }
}
