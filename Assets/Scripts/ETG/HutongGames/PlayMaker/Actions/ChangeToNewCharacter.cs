// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ChangeToNewCharacter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Only use this in the Foyer!")]
[ActionCategory(".NPCs")]
public class ChangeToNewCharacter : FsmStateAction
{
  public string PlayerPrefabPath;

  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    GameManager.Instance.StartCoroutine(this.HandleCharacterChange());
  }

  [DebuggerHidden]
  private IEnumerator HandleCharacterChange()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ChangeToNewCharacter.<HandleCharacterChange>c__Iterator0()
    {
      _this = this
    };
  }

  private PlayerController GeneratePlayer()
  {
    PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
    Vector3 position = primaryPlayer.transform.position;
    Object.Destroy((Object) primaryPlayer.gameObject);
    GameManager.Instance.ClearPrimaryPlayer();
    GameManager.PlayerPrefabForNewGame = (GameObject) BraveResources.Load(this.PlayerPrefabPath);
    GameStatsManager.Instance.BeginNewSession(GameManager.PlayerPrefabForNewGame.GetComponent<PlayerController>());
    PlayerController player = (PlayerController) null;
    if ((Object) player == (Object) null)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(GameManager.PlayerPrefabForNewGame, position, Quaternion.identity);
      GameManager.PlayerPrefabForNewGame = (GameObject) null;
      gameObject.SetActive(true);
      player = gameObject.GetComponent<PlayerController>();
    }
    FoyerCharacterSelectFlag component = this.Owner.GetComponent<FoyerCharacterSelectFlag>();
    if ((bool) (Object) component && component.IsAlternateCostume)
      player.SwapToAlternateCostume();
    GameManager.Instance.PrimaryPlayer = player;
    player.PlayerIDX = 0;
    return player;
  }
}
