// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetRandomObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[HutongGames.PlayMaker.Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
public class GetRandomObject : FsmStateAction
{
  [UIHint(UIHint.Tag)]
  public FsmString withTag;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmGameObject storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.withTag = (FsmString) "Untagged";
    this.storeResult = (FsmGameObject) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetRandomObject();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetRandomObject();

  private void DoGetRandomObject()
  {
    GameObject[] gameObjectArray = !(this.withTag.Value != "Untagged") ? (GameObject[]) Object.FindObjectsOfType(typeof (GameObject)) : GameObject.FindGameObjectsWithTag(this.withTag.Value);
    if (gameObjectArray.Length > 0)
      this.storeResult.Value = gameObjectArray[Random.Range(0, gameObjectArray.Length)];
    else
      this.storeResult.Value = (GameObject) null;
  }
}
