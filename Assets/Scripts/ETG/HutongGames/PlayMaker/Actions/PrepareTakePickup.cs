// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PrepareTakePickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class PrepareTakePickup : FsmStateAction
  {
    public int TargetPickupIndex;

    public override void OnEnter()
    {
      PickupObject byId = PickupObjectDatabase.GetById(this.TargetPickupIndex);
      FsmString fsmString = this.Fsm.Variables.GetFsmString("npcReplacementString");
      EncounterTrackable component = byId.GetComponent<EncounterTrackable>();
      if (fsmString != null && (Object) component != (Object) null)
        fsmString.Value = component.journalData.GetPrimaryDisplayName();
      this.Finish();
    }
  }
}
