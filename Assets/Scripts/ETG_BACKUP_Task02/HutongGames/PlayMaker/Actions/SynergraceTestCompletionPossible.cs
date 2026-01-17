// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SynergraceTestCompletionPossible
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends Events based on synergy completion possibility.")]
[ActionCategory(".Brave")]
public class SynergraceTestCompletionPossible : BraveFsmStateAction
{
  public SynergraceTestCompletionPossible.SuccessType successType;
  [HutongGames.PlayMaker.Tooltip("The event to send if the proceeding tests all pass.")]
  public FsmEvent Event;
  [HutongGames.PlayMaker.Tooltip("The name of the mode to set 'currentMode' to if the proceeding tests all pass.")]
  public FsmString mode;
  public FsmBool everyFrame;
  [NonSerialized]
  public GameObject SelectedPickupGameObject;
  private bool m_success;

  public bool Success => this.m_success;

  public override void Reset()
  {
    this.successType = SynergraceTestCompletionPossible.SuccessType.SetMode;
    this.Event = (FsmEvent) null;
    this.mode = (FsmString) string.Empty;
  }

  public override void OnEnter()
  {
    this.DoCheck();
    if (this.everyFrame.Value)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoCheck();

  private void DoCheck()
  {
    this.m_success = false;
    GenericLootTable tableToUse1 = (double) UnityEngine.Random.value >= 0.5 ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
    GenericLootTable tableToUse2 = !((UnityEngine.Object) tableToUse1 == (UnityEngine.Object) GameManager.Instance.RewardManager.GunsLootTable) ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
    SynercacheManager.UseCachedSynergyIDs = true;
    GameObject gameObject = GameManager.Instance.RewardManager.GetItemForPlayer(GameManager.Instance.BestActivePlayer, tableToUse1, PickupObject.ItemQuality.A, (List<GameObject>) null, forceSynergyCompletion: true);
    if ((bool) (UnityEngine.Object) gameObject)
    {
      PickupObject component = gameObject.GetComponent<PickupObject>();
      bool usesStartingItem = false;
      if (!(bool) (UnityEngine.Object) component || !RewardManager.AnyPlayerHasItemInSynergyContainingOtherItem(component, ref usesStartingItem))
        gameObject = (GameObject) null;
    }
    if (!(bool) (UnityEngine.Object) gameObject)
      gameObject = GameManager.Instance.RewardManager.GetItemForPlayer(GameManager.Instance.BestActivePlayer, tableToUse2, PickupObject.ItemQuality.A, (List<GameObject>) null, forceSynergyCompletion: true);
    if ((bool) (UnityEngine.Object) gameObject)
    {
      PickupObject component = gameObject.GetComponent<PickupObject>();
      bool usesStartingItem = false;
      if ((bool) (UnityEngine.Object) component && RewardManager.AnyPlayerHasItemInSynergyContainingOtherItem(component, ref usesStartingItem))
      {
        this.m_success = true;
        this.SelectedPickupGameObject = gameObject;
      }
    }
    SynercacheManager.UseCachedSynergyIDs = false;
    if (!this.m_success)
      return;
    if (this.successType == SynergraceTestCompletionPossible.SuccessType.SendEvent)
      this.Fsm.Event(this.Event);
    else if (this.successType == SynergraceTestCompletionPossible.SuccessType.SetMode)
      this.Fsm.Variables.GetFsmString("currentMode").Value = this.mode.Value;
    this.Finish();
  }

  public enum SuccessType
  {
    SetMode,
    SendEvent,
  }
}
