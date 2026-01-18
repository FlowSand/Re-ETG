using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Pause an iTween action.")]
  [ActionCategory("iTween")]
  public class iTweenPause : FsmStateAction
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
        iTween.Pause();
      else if (this.inScene)
      {
        iTween.Pause(Enum.GetName(typeof (iTweenFSMType), (object) this.iTweenType));
      }
      else
      {
        GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
          return;
        iTween.Pause(ownerDefaultTarget, Enum.GetName(typeof (iTweenFSMType), (object) this.iTweenType), this.includeChildren);
      }
    }
  }
}
