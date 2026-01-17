// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FindClosest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[HutongGames.PlayMaker.Tooltip("Finds the closest object to the specified Game Object.\nOptionally filter by Tag and Visibility.")]
public class FindClosest : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The GameObject to measure from.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Only consider objects with this Tag. NOTE: It's generally a lot quicker to find objects with a Tag!")]
  [UIHint(UIHint.Tag)]
  [RequiredField]
  public FsmString withTag;
  [HutongGames.PlayMaker.Tooltip("If checked, ignores the object that owns this FSM.")]
  public FsmBool ignoreOwner;
  [HutongGames.PlayMaker.Tooltip("Only consider objects visible to the camera.")]
  public FsmBool mustBeVisible;
  [HutongGames.PlayMaker.Tooltip("Store the closest object.")]
  [UIHint(UIHint.Variable)]
  public FsmGameObject storeObject;
  [HutongGames.PlayMaker.Tooltip("Store the distance to the closest object.")]
  [UIHint(UIHint.Variable)]
  public FsmFloat storeDistance;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.withTag = (FsmString) "Untagged";
    this.ignoreOwner = (FsmBool) true;
    this.mustBeVisible = (FsmBool) false;
    this.storeObject = (FsmGameObject) null;
    this.storeDistance = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoFindClosest();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoFindClosest();

  private void DoFindClosest()
  {
    GameObject gameObject1 = this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner;
    GameObject[] gameObjectArray = string.IsNullOrEmpty(this.withTag.Value) || this.withTag.Value == "Untagged" ? (GameObject[]) Object.FindObjectsOfType(typeof (GameObject)) : GameObject.FindGameObjectsWithTag(this.withTag.Value);
    GameObject gameObject2 = (GameObject) null;
    float f = float.PositiveInfinity;
    foreach (GameObject go in gameObjectArray)
    {
      if ((!this.ignoreOwner.Value || !((Object) go == (Object) this.Owner)) && (!this.mustBeVisible.Value || ActionHelpers.IsVisible(go)))
      {
        float sqrMagnitude = (gameObject1.transform.position - go.transform.position).sqrMagnitude;
        if ((double) sqrMagnitude < (double) f)
        {
          f = sqrMagnitude;
          gameObject2 = go;
        }
      }
    }
    this.storeObject.Value = gameObject2;
    if (this.storeDistance.IsNone)
      return;
    this.storeDistance.Value = Mathf.Sqrt(f);
  }
}
