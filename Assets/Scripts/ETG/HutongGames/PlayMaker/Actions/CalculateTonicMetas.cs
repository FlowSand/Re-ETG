// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CalculateTonicMetas
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  public class CalculateTonicMetas : FsmStateAction
  {
    public override void OnEnter()
    {
      int num = Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY));
      FsmInt fsmInt = this.Fsm.Variables.FindFsmInt("npcNumber1");
      FsmFloat fsmFloat = this.Fsm.Variables.FindFsmFloat("costFloat");
      fsmInt.Value = Mathf.RoundToInt(((float) num * 0.9f).Quantize(50f));
      fsmFloat.Value = (float) fsmInt.Value;
      this.Finish();
    }
  }
}
