// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenStop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Stop an iTween action.")]
[ActionCategory("iTween")]
public class iTweenStop : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  public FsmString id;
  public iTweenFSMType iTweenType;
  public bool includeChildren;
  public bool inScene;

  public override void Reset()
  {
    FsmString fsmString = new FsmString();
    fsmString.UseVariable = true;
    this.id = fsmString;
    this.iTweenType = iTweenFSMType.all;
    this.includeChildren = false;
    this.inScene = false;
  }

  public override void OnEnter()
  {
    base.OnEnter();
    this.DoiTween();
    this.Finish();
  }

  private void DoiTween()
  {
    if (this.id.IsNone)
    {
      if (this.iTweenType == iTweenFSMType.all)
        iTween.Stop();
      else if (this.inScene)
      {
        iTween.Stop(Enum.GetName(typeof (iTweenFSMType), (object) this.iTweenType));
      }
      else
      {
        GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
          return;
        iTween.Stop(ownerDefaultTarget, Enum.GetName(typeof (iTweenFSMType), (object) this.iTweenType), this.includeChildren);
      }
    }
    else
      iTween.StopByName(this.id.Value);
  }
}
