// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.LookAt2dGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a Target.")]
  [ActionCategory(ActionCategory.Transform)]
  public class LookAt2dGameObject : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The GameObject to Look At.")]
    public FsmGameObject targetObject;
    [HutongGames.PlayMaker.Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
    public FsmFloat rotationOffset;
    [HutongGames.PlayMaker.Tooltip("Draw a debug line from the GameObject to the Target.")]
    [Title("Draw Debug Line")]
    public FsmBool debug;
    [HutongGames.PlayMaker.Tooltip("Color to use for the debug line.")]
    public FsmColor debugLineColor;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame = true;
    private GameObject go;
    private GameObject goTarget;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.targetObject = (FsmGameObject) null;
      this.debug = (FsmBool) false;
      this.debugLineColor = (FsmColor) Color.green;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.DoLookAt();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoLookAt();

    private void DoLookAt()
    {
      this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      this.goTarget = this.targetObject.Value;
      if ((Object) this.go == (Object) null || this.targetObject == null)
        return;
      Vector3 vector3 = this.goTarget.transform.position - this.go.transform.position;
      vector3.Normalize();
      this.go.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(vector3.y, vector3.x) * 57.29578f - this.rotationOffset.Value);
      if (!this.debug.Value)
        return;
      Debug.DrawLine(this.go.transform.position, this.goTarget.transform.position, this.debugLineColor.Value);
    }
  }
}
