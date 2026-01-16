// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MoveObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Move a GameObject to another GameObject. Works like iTween Move To, but with better performance.")]
[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4758.0")]
[ActionCategory(ActionCategory.Transform)]
public class MoveObject : EaseFsmAction
{
  [RequiredField]
  public FsmOwnerDefault objectToMove;
  [RequiredField]
  public FsmGameObject destination;
  private FsmVector3 fromValue;
  private FsmVector3 toVector;
  private FsmVector3 fromVector;
  private bool finishInNextStep;

  public override void Reset()
  {
    base.Reset();
    this.fromValue = (FsmVector3) null;
    this.toVector = (FsmVector3) null;
    this.finishInNextStep = false;
    this.fromVector = (FsmVector3) null;
  }

  public override void OnEnter()
  {
    base.OnEnter();
    this.fromVector = (FsmVector3) this.Fsm.GetOwnerDefaultTarget(this.objectToMove).transform.position;
    this.toVector = (FsmVector3) this.destination.Value.transform.position;
    this.fromFloats = new float[3];
    this.fromFloats[0] = this.fromVector.Value.x;
    this.fromFloats[1] = this.fromVector.Value.y;
    this.fromFloats[2] = this.fromVector.Value.z;
    this.toFloats = new float[3];
    this.toFloats[0] = this.toVector.Value.x;
    this.toFloats[1] = this.toVector.Value.y;
    this.toFloats[2] = this.toVector.Value.z;
    this.resultFloats = new float[3];
    this.resultFloats[0] = this.fromVector.Value.x;
    this.resultFloats[1] = this.fromVector.Value.y;
    this.resultFloats[2] = this.fromVector.Value.z;
    this.finishInNextStep = false;
  }

  public override void OnUpdate()
  {
    base.OnUpdate();
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.objectToMove);
    ownerDefaultTarget.transform.position = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
    if (this.finishInNextStep)
    {
      this.Finish();
      if (this.finishEvent != null)
        this.Fsm.Event(this.finishEvent);
    }
    if (!this.finishAction || this.finishInNextStep)
      return;
    ownerDefaultTarget.transform.position = new Vector3(!this.reverse.IsNone ? (!this.reverse.Value ? this.toVector.Value.x : this.fromValue.Value.x) : this.toVector.Value.x, !this.reverse.IsNone ? (!this.reverse.Value ? this.toVector.Value.y : this.fromValue.Value.y) : this.toVector.Value.y, !this.reverse.IsNone ? (!this.reverse.Value ? this.toVector.Value.z : this.fromValue.Value.z) : this.toVector.Value.z);
    this.finishInNextStep = true;
  }
}
