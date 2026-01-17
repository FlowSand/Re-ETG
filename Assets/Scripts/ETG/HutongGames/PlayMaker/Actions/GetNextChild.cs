// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetNextChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Each time this action is called it gets the next child of a GameObject. This lets you quickly loop through all the children of an object to perform actions on them. NOTE: To find a specific child use Find Child.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class GetNextChild : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The parent GameObject. Note, if GameObject changes, this action will reset and start again at the first child.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the next child in a GameObject variable.")]
    [RequiredField]
    public FsmGameObject storeNextChild;
    [HutongGames.PlayMaker.Tooltip("Event to send to get the next child.")]
    public FsmEvent loopEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send when there are no more children.")]
    public FsmEvent finishedEvent;
    private GameObject go;
    private int nextChildIndex;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.storeNextChild = (FsmGameObject) null;
      this.loopEvent = (FsmEvent) null;
      this.finishedEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this.DoGetNextChild(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
      this.Finish();
    }

    private void DoGetNextChild(GameObject parent)
    {
      if ((Object) parent == (Object) null)
        return;
      if ((Object) this.go != (Object) parent)
      {
        this.go = parent;
        this.nextChildIndex = 0;
      }
      if (this.nextChildIndex >= this.go.transform.childCount)
      {
        this.nextChildIndex = 0;
        this.Fsm.Event(this.finishedEvent);
      }
      else
      {
        this.storeNextChild.Value = parent.transform.GetChild(this.nextChildIndex).gameObject;
        if (this.nextChildIndex >= this.go.transform.childCount)
        {
          this.nextChildIndex = 0;
          this.Fsm.Event(this.finishedEvent);
        }
        else
        {
          ++this.nextChildIndex;
          if (this.loopEvent == null)
            return;
          this.Fsm.Event(this.loopEvent);
        }
      }
    }
  }
}
