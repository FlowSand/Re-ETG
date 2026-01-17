// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkDestroy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Destroy the object across the network.\n\nThe object is destroyed locally and remotely.\n\nOptionally remove any RPCs accociated with the object.")]
[ActionCategory(ActionCategory.Network)]
public class NetworkDestroy : ComponentAction<NetworkView>
{
  [HutongGames.PlayMaker.Tooltip("The Game Object to destroy.\nNOTE: The Game Object must have a NetworkView attached.")]
  [RequiredField]
  [CheckForComponent(typeof (NetworkView))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Remove all RPC calls associated with the Game Object.")]
  public FsmBool removeRPCs;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.removeRPCs = (FsmBool) true;
  }

  public override void OnEnter()
  {
    this.DoDestroy();
    this.Finish();
  }

  private void DoDestroy()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    if (this.removeRPCs.Value)
      Network.RemoveRPCs(this.networkView.owner);
    Network.DestroyPlayerObjects(this.networkView.owner);
  }
}
