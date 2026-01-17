// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGravity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the gravity vector, or individual axis.")]
  [ActionCategory(ActionCategory.Physics)]
  public class SetGravity : FsmStateAction
  {
    public FsmVector3 vector;
    public FsmFloat x;
    public FsmFloat y;
    public FsmFloat z;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector = (FsmVector3) null;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.x = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.y = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.z = fsmFloat3;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetGravity();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetGravity();

    private void DoSetGravity()
    {
      Vector3 vector3 = this.vector.Value;
      if (!this.x.IsNone)
        vector3.x = this.x.Value;
      if (!this.y.IsNone)
        vector3.y = this.y.Value;
      if (!this.z.IsNone)
        vector3.z = this.z.Value;
      Physics.gravity = vector3;
    }
  }
}
