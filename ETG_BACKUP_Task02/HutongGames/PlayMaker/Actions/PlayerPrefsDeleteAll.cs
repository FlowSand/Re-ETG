// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayerPrefsDeleteAll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("PlayerPrefs")]
[Tooltip("Removes all keys and values from the preferences. Use with caution.")]
public class PlayerPrefsDeleteAll : FsmStateAction
{
  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    PlayerPrefs.DeleteAll();
    this.Finish();
  }
}
