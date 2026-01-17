// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkSetMinimumAllocatableViewIDs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the minimum number of ViewID numbers in the ViewID pool given to clients by the server. The default value is 100.\n\nThe ViewID pools are given to each player as he connects and are refreshed with new numbers if the player runs out. The server and clients should be in sync regarding this value.\n\nSetting this higher only on the server has the effect that he sends more view ID numbers to clients, than they really want.\n\nSetting this higher only on clients means they request more view IDs more often, for example twice in a row, as the pools received from the server don't contain enough numbers. ")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkSetMinimumAllocatableViewIDs : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The minimum number of ViewID numbers in the ViewID pool given to clients by the server. The default value is 100.")]
    public FsmInt minimumViewIDs;

    public override void Reset() => this.minimumViewIDs = (FsmInt) 100;

    public override void OnEnter()
    {
      Network.minimumAllocatableViewIDs = this.minimumViewIDs.Value;
      this.Finish();
    }
  }
}
