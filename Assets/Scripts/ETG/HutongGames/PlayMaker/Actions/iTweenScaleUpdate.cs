// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenScaleUpdate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("CSimilar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
  [ActionCategory("iTween")]
  public class iTweenScaleUpdate : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Scale To a transform scale.")]
    public FsmGameObject transformScale;
    [HutongGames.PlayMaker.Tooltip("A scale vector the GameObject will animate To.")]
    public FsmVector3 vectorScale;
    [HutongGames.PlayMaker.Tooltip("The time in seconds the animation will take to complete. If transformScale is set, this is used as an offset.")]
    public FsmFloat time;
    private Hashtable hash;
    private GameObject go;

    public override void Reset()
    {
      FsmGameObject fsmGameObject = new FsmGameObject();
      fsmGameObject.UseVariable = true;
      this.transformScale = fsmGameObject;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.vectorScale = fsmVector3;
      this.time = (FsmFloat) 1f;
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
        if (this.transformScale.IsNone)
          this.hash.Add((object) "scale", (object) (!this.vectorScale.IsNone ? this.vectorScale.Value : Vector3.zero));
        else if (this.vectorScale.IsNone)
          this.hash.Add((object) "scale", (object) this.transformScale.Value.transform);
        else
          this.hash.Add((object) "scale", (object) (this.transformScale.Value.transform.localScale + this.vectorScale.Value));
        this.hash.Add((object) "time", (object) (float) (!this.time.IsNone ? (double) this.time.Value : 1.0));
        this.DoiTween();
      }
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
      this.hash.Remove((object) "scale");
      if (this.transformScale.IsNone)
        this.hash.Add((object) "scale", (object) (!this.vectorScale.IsNone ? this.vectorScale.Value : Vector3.zero));
      else if (this.vectorScale.IsNone)
        this.hash.Add((object) "scale", (object) this.transformScale.Value.transform);
      else
        this.hash.Add((object) "scale", (object) (this.transformScale.Value.transform.localScale + this.vectorScale.Value));
      this.DoiTween();
    }

    private void DoiTween() => iTween.ScaleUpdate(this.go, this.hash);
  }
}
