// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkInstantiate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Creates a Game Object on all clients in a network game.")]
[ActionCategory(ActionCategory.Network)]
public class NetworkInstantiate : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The prefab will be instanted on all clients in the game.")]
  [RequiredField]
  public FsmGameObject prefab;
  [HutongGames.PlayMaker.Tooltip("Optional Spawn Point.")]
  public FsmGameObject spawnPoint;
  [HutongGames.PlayMaker.Tooltip("Spawn Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
  public FsmVector3 position;
  [HutongGames.PlayMaker.Tooltip("Spawn Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
  public FsmVector3 rotation;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Optionally store the created object.")]
  public FsmGameObject storeObject;
  [HutongGames.PlayMaker.Tooltip("Usually 0. The group number allows you to group together network messages which allows you to filter them if so desired.")]
  public FsmInt networkGroup;

  public override void Reset()
  {
    this.prefab = (FsmGameObject) null;
    this.spawnPoint = (FsmGameObject) null;
    FsmVector3 fsmVector3_1 = new FsmVector3();
    fsmVector3_1.UseVariable = true;
    this.position = fsmVector3_1;
    FsmVector3 fsmVector3_2 = new FsmVector3();
    fsmVector3_2.UseVariable = true;
    this.rotation = fsmVector3_2;
    this.storeObject = (FsmGameObject) null;
    this.networkGroup = (FsmInt) 0;
  }

  public override void OnEnter()
  {
    GameObject prefab = this.prefab.Value;
    if ((Object) prefab != (Object) null)
    {
      Vector3 position = Vector3.zero;
      Vector3 euler = Vector3.up;
      if ((Object) this.spawnPoint.Value != (Object) null)
      {
        position = this.spawnPoint.Value.transform.position;
        if (!this.position.IsNone)
          position += this.position.Value;
        euler = this.rotation.IsNone ? this.spawnPoint.Value.transform.eulerAngles : this.rotation.Value;
      }
      else
      {
        if (!this.position.IsNone)
          position = this.position.Value;
        if (!this.rotation.IsNone)
          euler = this.rotation.Value;
      }
      this.storeObject.Value = (GameObject) Network.Instantiate((Object) prefab, position, Quaternion.Euler(euler), this.networkGroup.Value);
    }
    this.Finish();
  }
}
