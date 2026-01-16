// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MassiveSaveFlagSwitchboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".Brave")]
[Tooltip("Sends Events based on the value of a player save flag.")]
public class MassiveSaveFlagSwitchboard : FsmStateAction
{
  public MassiveSaveFlagEntry[] entries;

  public override void Reset() => this.entries = new MassiveSaveFlagEntry[0];

  public override string ErrorCheck() => string.Empty;

  public override void OnEnter()
  {
    this.DoCheck();
    this.Finish();
  }

  private void DoCheck()
  {
    for (int index = 0; index < this.entries.Length; ++index)
    {
      if (GameStatsManager.Instance.GetFlag(this.entries[index].RequiredFlag) == this.entries[index].RequiredFlagState && !GameStatsManager.Instance.GetFlag(this.entries[index].CompletedFlag) && (this.entries[index].CompletedFlag != GungeonFlags.CREST_NPC_SGDQ2018 || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH))
      {
        this.Fsm.Variables.GetFsmString("currentMode").Value = this.entries[index].mode;
        break;
      }
    }
  }

  public enum SuccessType
  {
    SetMode,
    SendEvent,
  }
}
