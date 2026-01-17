// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SmoothFollowAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Action version of Unity's Smooth Follow script.")]
  [ActionCategory(ActionCategory.Transform)]
  public class SmoothFollowAction : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The game object to control. E.g. The camera.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The GameObject to follow.")]
    public FsmGameObject targetObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The distance in the x-z plane to the target.")]
    public FsmFloat distance;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The height we want the camera to be above the target")]
    public FsmFloat height;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("How much to dampen height movement.")]
    public FsmFloat heightDamping;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("How much to dampen rotation changes.")]
    public FsmFloat rotationDamping;
    private GameObject cachedObject;
    private Transform myTransform;
    private GameObject cachedTarget;
    private Transform targetTransform;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.targetObject = (FsmGameObject) null;
      this.distance = (FsmFloat) 10f;
      this.height = (FsmFloat) 5f;
      this.heightDamping = (FsmFloat) 2f;
      this.rotationDamping = (FsmFloat) 3f;
    }

    public override void OnLateUpdate()
    {
      if ((Object) this.targetObject.Value == (Object) null)
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) this.cachedObject != (Object) ownerDefaultTarget)
      {
        this.cachedObject = ownerDefaultTarget;
        this.myTransform = ownerDefaultTarget.transform;
      }
      if ((Object) this.cachedTarget != (Object) this.targetObject.Value)
      {
        this.cachedTarget = this.targetObject.Value;
        this.targetTransform = this.cachedTarget.transform;
      }
      float y1 = this.targetTransform.eulerAngles.y;
      float b = this.targetTransform.position.y + this.height.Value;
      float y2 = this.myTransform.eulerAngles.y;
      float y3 = this.myTransform.position.y;
      float y4 = Mathf.LerpAngle(y2, y1, this.rotationDamping.Value * Time.deltaTime);
      float y5 = Mathf.Lerp(y3, b, this.heightDamping.Value * Time.deltaTime);
      Quaternion quaternion = Quaternion.Euler(0.0f, y4, 0.0f);
      this.myTransform.position = this.targetTransform.position;
      this.myTransform.position -= quaternion * Vector3.forward * this.distance.Value;
      this.myTransform.position = new Vector3(this.myTransform.position.x, y5, this.myTransform.position.z);
      this.myTransform.LookAt(this.targetTransform);
    }
  }
}
