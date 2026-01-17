// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerClearHostList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Clear the host list which was received by MasterServer Request Host List")]
  public class MasterServerClearHostList : FsmStateAction
  {
    public override void OnEnter()
    {
      MasterServer.ClearHostList();
      this.Finish();
    }
  }
}
