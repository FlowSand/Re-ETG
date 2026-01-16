// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerGetHostCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the number of hosts on the master server.\n\nUse MasterServer Get Host Data to get host data at a specific index.")]
[ActionCategory(ActionCategory.Network)]
public class MasterServerGetHostCount : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The number of hosts on the MasterServer.")]
  [RequiredField]
  public FsmInt count;

  public override void OnEnter()
  {
    this.count.Value = MasterServer.PollHostList().Length;
    this.Finish();
  }
}
