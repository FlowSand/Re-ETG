using System.Collections;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
  [ActionCategory("iTween")]
  public class iTweenRotateUpdate : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Rotate to a transform rotation.")]
    public FsmGameObject transformRotation;
    [HutongGames.PlayMaker.Tooltip("A rotation the GameObject will animate from.")]
    public FsmVector3 vectorRotation;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete. If transformRotation is set, this is used as an offset.")]
    public FsmFloat time;
    [HutongGames.PlayMaker.Tooltip("Whether to animate in local or world space.")]
    public Space space;
    private Hashtable hash;
    private GameObject go;

    public override void Reset()
    {
      FsmGameObject fsmGameObject = new FsmGameObject();
      fsmGameObject.UseVariable = true;
      this.transformRotation = fsmGameObject;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.vectorRotation = fsmVector3;
      this.time = (FsmFloat) 1f;
      this.space = Space.World;
    }

    public override void OnEnter()
    {
      this.hash = new Hashtable();
      this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) this.go == (Object) null)
      {
        this.Finish();
      }
      else
      {
        if (this.transformRotation.IsNone)
          this.hash.Add((object) "rotation", (object) (!this.vectorRotation.IsNone ? this.vectorRotation.Value : Vector3.zero));
        else if (this.vectorRotation.IsNone)
          this.hash.Add((object) "rotation", (object) this.transformRotation.Value.transform);
        else if (this.space == Space.World)
          this.hash.Add((object) "rotation", (object) (this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value));
        else
          this.hash.Add((object) "rotation", (object) (this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value));
        this.hash.Add((object) "time", (object) (float) (!this.time.IsNone ? (double) this.time.Value : 1.0));
        this.hash.Add((object) "islocal", (object) (this.space == Space.Self));
        this.DoiTween();
      }
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
      this.hash.Remove((object) "rotation");
      if (this.transformRotation.IsNone)
        this.hash.Add((object) "rotation", (object) (!this.vectorRotation.IsNone ? this.vectorRotation.Value : Vector3.zero));
      else if (this.vectorRotation.IsNone)
        this.hash.Add((object) "rotation", (object) this.transformRotation.Value.transform);
      else if (this.space == Space.World)
        this.hash.Add((object) "rotation", (object) (this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value));
      else
        this.hash.Add((object) "rotation", (object) (this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value));
      this.DoiTween();
    }

    private void DoiTween() => iTween.RotateUpdate(this.go, this.hash);
  }
}
