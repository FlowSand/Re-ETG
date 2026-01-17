// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CurrentGunIsDroppable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Toss the current gun into the gunper monper and (hopefully) get an upgrade.")]
[ActionCategory(".NPCs")]
public class CurrentGunIsDroppable : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Event to send if the player is in the foyer.")]
  public FsmEvent isTrue;
  [HutongGames.PlayMaker.Tooltip("Event to send if the player is not in the foyer.")]
  public FsmEvent isFalse;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.isTrue = (FsmEvent) null;
    this.isFalse = (FsmEvent) null;
    this.everyFrame = false;
  }

  private bool TestGun()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    bool flag = false;
    if ((bool) (Object) component && (bool) (Object) component.TalkingPlayer)
    {
      if (component.TalkingPlayer.CharacterUsesRandomGuns)
        return false;
      Gun currentGun = component.TalkingPlayer.CurrentGun;
      if ((bool) (Object) currentGun && currentGun.CanActuallyBeDropped(component.TalkingPlayer) && !currentGun.InfiniteAmmo)
        flag = true;
    }
    return flag;
  }

  public override void OnEnter()
  {
    this.Fsm.Event(!this.TestGun() ? this.isFalse : this.isTrue);
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.Fsm.Event(!this.TestGun() ? this.isFalse : this.isTrue);
}
