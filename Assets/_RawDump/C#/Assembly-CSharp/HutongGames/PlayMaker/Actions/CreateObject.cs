// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CreateObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionTarget(typeof (GameObject), "gameObject", true)]
[HutongGames.PlayMaker.Tooltip("Creates a Game Object, usually using a Prefab.")]
[ActionCategory(ActionCategory.GameObject)]
public class CreateObject : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("GameObject to create. Usually a Prefab.")]
  [RequiredField]
  public FsmGameObject gameObject;
  [HutongGames.PlayMaker.Tooltip("Optional Spawn Point.")]
  public FsmGameObject spawnPoint;
  [HutongGames.PlayMaker.Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
  public FsmVector3 position;
  [HutongGames.PlayMaker.Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
  public FsmVector3 rotation;
  [HutongGames.PlayMaker.Tooltip("Optionally store the created object.")]
  [UIHint(UIHint.Variable)]
  public FsmGameObject storeObject;
  [HutongGames.PlayMaker.Tooltip("Use Network.Instantiate to create a Game Object on all clients in a networked game.")]
  public FsmBool networkInstantiate;
  [HutongGames.PlayMaker.Tooltip("Usually 0. The group number allows you to group together network messages which allows you to filter them if so desired.")]
  public FsmInt networkGroup;

  public override void Reset()
  {
    this.gameObject = (FsmGameObject) null;
    this.spawnPoint = (FsmGameObject) null;
    FsmVector3 fsmVector3_1 = new FsmVector3();
    fsmVector3_1.UseVariable = true;
    this.position = fsmVector3_1;
    FsmVector3 fsmVector3_2 = new FsmVector3();
    fsmVector3_2.UseVariable = true;
    this.rotation = fsmVector3_2;
    this.storeObject = (FsmGameObject) null;
    this.networkInstantiate = (FsmBool) false;
    this.networkGroup = (FsmInt) 0;
  }

  public override void OnEnter()
  {
    GameObject gameObject = this.gameObject.Value;
    if ((Object) gameObject != (Object) null)
    {
      Vector3 position = Vector3.zero;
      Vector3 euler = Vector3.zero;
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
      this.storeObject.Value = this.networkInstantiate.Value ? (GameObject) Network.Instantiate((Object) gameObject, position, Quaternion.Euler(euler), this.networkGroup.Value) : Object.Instantiate<GameObject>(gameObject, position, Quaternion.Euler(euler));
    }
    this.Finish();
  }
}
