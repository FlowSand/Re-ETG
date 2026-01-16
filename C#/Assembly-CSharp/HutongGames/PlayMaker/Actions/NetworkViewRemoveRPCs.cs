// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkViewRemoveRPCs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[HutongGames.PlayMaker.Tooltip("Remove the RPC function calls accociated with a Game Object.\n\nNOTE: The Game Object must have a NetworkView component attached.")]
public class NetworkViewRemoveRPCs : ComponentAction<NetworkView>
{
  [HutongGames.PlayMaker.Tooltip("Remove the RPC function calls accociated with this Game Object.\n\nNOTE: The GameObject must have a NetworkView component attached.")]
  [RequiredField]
  [CheckForComponent(typeof (NetworkView))]
  public FsmOwnerDefault gameObject;

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    this.DoRemoveRPCsFromViewID();
    this.Finish();
  }

  private void DoRemoveRPCsFromViewID()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    Network.RemoveRPCs(this.networkView.viewID);
  }
}
