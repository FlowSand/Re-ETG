// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PreloadBossBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".NPCs")]
public class PreloadBossBar : FsmStateAction
{
  public override void OnEnter()
  {
    GameUIRoot.Instance.bossController.ForceUpdateBossHealth(100f, 100f, StringTableManager.GetEnemiesString("#MANFRED_ENCNAME"));
    GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_Boss_Theme_Gull", this.Owner.gameObject);
    this.Finish();
  }
}
