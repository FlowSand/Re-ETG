// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vector2RotateTowards
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Rotates a Vector2 direction from Current towards Target.")]
[ActionCategory(ActionCategory.Vector2)]
public class Vector2RotateTowards : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The current direction. This will be the result of the rotation as well.")]
  [RequiredField]
  public FsmVector2 currentDirection;
  [HutongGames.PlayMaker.Tooltip("The direction to reach")]
  [RequiredField]
  public FsmVector2 targetDirection;
  [HutongGames.PlayMaker.Tooltip("Rotation speed in degrees per second")]
  [RequiredField]
  public FsmFloat rotateSpeed;
  private Vector3 current;
  private Vector3 target;

  public override void Reset()
  {
    FsmVector2 fsmVector2_1 = new FsmVector2();
    fsmVector2_1.UseVariable = true;
    this.currentDirection = fsmVector2_1;
    FsmVector2 fsmVector2_2 = new FsmVector2();
    fsmVector2_2.UseVariable = true;
    this.targetDirection = fsmVector2_2;
    this.rotateSpeed = (FsmFloat) 360f;
  }

  public override void OnEnter()
  {
    this.current = new Vector3(this.currentDirection.Value.x, this.currentDirection.Value.y, 0.0f);
    this.target = new Vector3(this.targetDirection.Value.x, this.targetDirection.Value.y, 0.0f);
  }

  public override void OnUpdate()
  {
    this.current.x = this.currentDirection.Value.x;
    this.current.y = this.currentDirection.Value.y;
    this.current = Vector3.RotateTowards(this.current, this.target, this.rotateSpeed.Value * ((float) Math.PI / 180f) * Time.deltaTime, 1000f);
    this.currentDirection.Value = new Vector2(this.current.x, this.current.y);
  }
}
