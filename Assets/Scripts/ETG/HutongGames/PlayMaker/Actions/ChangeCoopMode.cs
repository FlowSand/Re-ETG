// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ChangeCoopMode
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
public class ChangeCoopMode : FsmStateAction
{
  public string PlayerPrefabPath;
  public bool TargetCoopMode = true;
  public bool IsTestCoopValid;
  public FsmEvent IfCoopValidEvent;

  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    if (this.IsTestCoopValid)
      this.Fsm.Event(this.IfCoopValidEvent);
    else
      this.Fsm.GameObject.GetComponent<TalkDoerLite>().StartCoroutine(this.HandleCharacterChange());
  }

  [DebuggerHidden]
  private IEnumerator HandleCharacterChange()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ChangeCoopMode.<HandleCharacterChange>c__Iterator0()
    {
      _this = this
    };
  }

  private PlayerController GeneratePlayer()
  {
    if ((Object) GameManager.Instance.SecondaryPlayer != (Object) null)
      return GameManager.Instance.SecondaryPlayer;
    GameManager.Instance.ClearSecondaryPlayer();
    GameManager.LastUsedCoopPlayerPrefab = (GameObject) BraveResources.Load(this.PlayerPrefabPath);
    PlayerController player = (PlayerController) null;
    if ((Object) player == (Object) null)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(GameManager.LastUsedCoopPlayerPrefab, this.Fsm.GameObject.transform.position, Quaternion.identity);
      gameObject.SetActive(true);
      player = gameObject.GetComponent<PlayerController>();
    }
    FoyerCharacterSelectFlag component = this.Owner.GetComponent<FoyerCharacterSelectFlag>();
    if ((bool) (Object) component && component.IsAlternateCostume)
      player.SwapToAlternateCostume();
    GameManager.Instance.SecondaryPlayer = player;
    player.PlayerIDX = 1;
    return player;
  }
}
