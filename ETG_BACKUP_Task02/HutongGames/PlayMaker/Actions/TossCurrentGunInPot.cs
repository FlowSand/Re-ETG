// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TossCurrentGunInPot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".NPCs")]
[Tooltip("Toss the current gun into the witches pot and (hopefully) get an upgrade.")]
public class TossCurrentGunInPot : FsmStateAction
{
  public FsmEvent SuccessEvent;
  private WitchCauldronController m_cauldron;

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    this.m_cauldron = component.transform.parent.GetComponent<WitchCauldronController>();
    if (this.m_cauldron.TossPlayerEquippedGun(component.TalkingPlayer))
      this.Fsm.Event(this.SuccessEvent);
    this.Finish();
  }

  public override void OnUpdate()
  {
    if (this.m_cauldron.IsGunInPot)
      return;
    this.Finish();
  }
}
