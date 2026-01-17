// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetRandomRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets Random Rotation for a Game Object. Uncheck an axis to keep its current value.")]
[ActionCategory(ActionCategory.Transform)]
public class SetRandomRotation : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [RequiredField]
  public FsmBool x;
  [RequiredField]
  public FsmBool y;
  [RequiredField]
  public FsmBool z;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.x = (FsmBool) true;
    this.y = (FsmBool) true;
    this.z = (FsmBool) true;
  }

  public override void OnEnter()
  {
    this.DoRandomRotation();
    this.Finish();
  }

  private void DoRandomRotation()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
    float x = localEulerAngles.x;
    float y = localEulerAngles.y;
    float z = localEulerAngles.z;
    if (this.x.Value)
      x = (float) Random.Range(0, 360);
    if (this.y.Value)
      y = (float) Random.Range(0, 360);
    if (this.z.Value)
      z = (float) Random.Range(0, 360);
    ownerDefaultTarget.transform.localEulerAngles = new Vector3(x, y, z);
  }
}
