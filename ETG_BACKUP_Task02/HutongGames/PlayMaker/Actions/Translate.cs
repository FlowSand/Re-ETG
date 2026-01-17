// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Translate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Transform)]
[HutongGames.PlayMaker.Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
public class Translate : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The game object to translate.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("A translation vector. NOTE: You can override individual axis below.")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 vector;
  [HutongGames.PlayMaker.Tooltip("Translation along x axis.")]
  public FsmFloat x;
  [HutongGames.PlayMaker.Tooltip("Translation along y axis.")]
  public FsmFloat y;
  [HutongGames.PlayMaker.Tooltip("Translation along z axis.")]
  public FsmFloat z;
  [HutongGames.PlayMaker.Tooltip("Translate in local or world space.")]
  public Space space;
  [HutongGames.PlayMaker.Tooltip("Translate over one second")]
  public bool perSecond;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;
  [HutongGames.PlayMaker.Tooltip("Perform the translate in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
  public bool lateUpdate;
  [HutongGames.PlayMaker.Tooltip("Perform the translate in FixedUpdate. This is useful when working with rigid bodies and physics.")]
  public bool fixedUpdate;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
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
    this.space = Space.Self;
    this.perSecond = true;
    this.everyFrame = true;
    this.lateUpdate = false;
    this.fixedUpdate = false;
  }

  public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

  public override void OnEnter()
  {
    if (this.everyFrame || this.lateUpdate || this.fixedUpdate)
      return;
    this.DoTranslate();
    this.Finish();
  }

  public override void OnUpdate()
  {
    if (this.lateUpdate || this.fixedUpdate)
      return;
    this.DoTranslate();
  }

  public override void OnLateUpdate()
  {
    if (this.lateUpdate)
      this.DoTranslate();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnFixedUpdate()
  {
    if (this.fixedUpdate)
      this.DoTranslate();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  private void DoTranslate()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    Vector3 translation = !this.vector.IsNone ? this.vector.Value : new Vector3(this.x.Value, this.y.Value, this.z.Value);
    if (!this.x.IsNone)
      translation.x = this.x.Value;
    if (!this.y.IsNone)
      translation.y = this.y.Value;
    if (!this.z.IsNone)
      translation.z = this.z.Value;
    if (!this.perSecond)
      ownerDefaultTarget.transform.Translate(translation, this.space);
    else
      ownerDefaultTarget.transform.Translate(translation * Time.deltaTime, this.space);
  }
}
