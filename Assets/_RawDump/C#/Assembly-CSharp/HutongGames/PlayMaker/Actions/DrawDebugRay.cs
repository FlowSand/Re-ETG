// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DrawDebugRay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Debug)]
[HutongGames.PlayMaker.Tooltip("Draws a line from a Start point in a direction. Specify the start point as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.")]
public class DrawDebugRay : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Draw ray from a GameObject.")]
  public FsmGameObject fromObject;
  [HutongGames.PlayMaker.Tooltip("Draw ray from a world position, or local offset from GameObject if provided.")]
  public FsmVector3 fromPosition;
  [HutongGames.PlayMaker.Tooltip("Direction vector of ray.")]
  public FsmVector3 direction;
  [HutongGames.PlayMaker.Tooltip("The color of the ray.")]
  public FsmColor color;

  public override void Reset()
  {
    FsmGameObject fsmGameObject = new FsmGameObject();
    fsmGameObject.UseVariable = true;
    this.fromObject = fsmGameObject;
    FsmVector3 fsmVector3_1 = new FsmVector3();
    fsmVector3_1.UseVariable = true;
    this.fromPosition = fsmVector3_1;
    FsmVector3 fsmVector3_2 = new FsmVector3();
    fsmVector3_2.UseVariable = true;
    this.direction = fsmVector3_2;
    this.color = (FsmColor) Color.white;
  }

  public override void OnUpdate()
  {
    Debug.DrawRay(ActionHelpers.GetPosition(this.fromObject, this.fromPosition), this.direction.Value, this.color.Value);
  }
}
