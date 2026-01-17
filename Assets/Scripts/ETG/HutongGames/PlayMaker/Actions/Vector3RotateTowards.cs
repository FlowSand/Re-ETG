// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector3RotateTowards
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector3)]
  [HutongGames.PlayMaker.Tooltip("Rotates a Vector3 direction from Current towards Target.")]
  public class Vector3RotateTowards : FsmStateAction
  {
    [RequiredField]
    public FsmVector3 currentDirection;
    [RequiredField]
    public FsmVector3 targetDirection;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Rotation speed in degrees per second")]
    public FsmFloat rotateSpeed;
    [HutongGames.PlayMaker.Tooltip("Max Magnitude per second")]
    [RequiredField]
    public FsmFloat maxMagnitude;

    public override void Reset()
    {
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.currentDirection = fsmVector3_1;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.targetDirection = fsmVector3_2;
      this.rotateSpeed = (FsmFloat) 360f;
      this.maxMagnitude = (FsmFloat) 1f;
    }

    public override void OnUpdate()
    {
      this.currentDirection.Value = Vector3.RotateTowards(this.currentDirection.Value, this.targetDirection.Value, this.rotateSpeed.Value * ((float) Math.PI / 180f) * Time.deltaTime, this.maxMagnitude.Value);
    }
  }
}
