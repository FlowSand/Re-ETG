using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Debug)]
  [HutongGames.PlayMaker.Tooltip("Draws a line from a Start point to an End point. Specify the points as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.")]
  public class DrawDebugLine : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Draw line from a GameObject.")]
    public FsmGameObject fromObject;
    [HutongGames.PlayMaker.Tooltip("Draw line from a world position, or local offset from GameObject if provided.")]
    public FsmVector3 fromPosition;
    [HutongGames.PlayMaker.Tooltip("Draw line to a GameObject.")]
    public FsmGameObject toObject;
    [HutongGames.PlayMaker.Tooltip("Draw line to a world position, or local offset from GameObject if provided.")]
    public FsmVector3 toPosition;
    [HutongGames.PlayMaker.Tooltip("The color of the line.")]
    public FsmColor color;

    public override void Reset()
    {
      FsmGameObject fsmGameObject1 = new FsmGameObject();
      fsmGameObject1.UseVariable = true;
      this.fromObject = fsmGameObject1;
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.fromPosition = fsmVector3_1;
      FsmGameObject fsmGameObject2 = new FsmGameObject();
      fsmGameObject2.UseVariable = true;
      this.toObject = fsmGameObject2;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.toPosition = fsmVector3_2;
      this.color = (FsmColor) Color.white;
    }

    public override void OnUpdate()
    {
      Debug.DrawLine(ActionHelpers.GetPosition(this.fromObject, this.fromPosition), ActionHelpers.GetPosition(this.toObject, this.toPosition), this.color.Value);
    }
  }
}
