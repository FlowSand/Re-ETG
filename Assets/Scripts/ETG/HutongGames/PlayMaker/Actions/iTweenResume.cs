// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.iTweenResume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("iTween")]
  [HutongGames.PlayMaker.Tooltip("Resume an iTween action.")]
  public class iTweenResume : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public iTweenFSMType iTweenType;
    public bool includeChildren;
    public bool inScene;

    public override void Reset()
    {
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
      if (this.iTweenType == iTweenFSMType.all)
        iTween.Resume();
      else if (this.inScene)
      {
        iTween.Resume(Enum.GetName(typeof (iTweenFSMType), (object) this.iTweenType));
      }
      else
      {
        GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
          return;
        iTween.Resume(ownerDefaultTarget, Enum.GetName(typeof (iTweenFSMType), (object) this.iTweenType), this.includeChildren);
      }
    }
  }
}
