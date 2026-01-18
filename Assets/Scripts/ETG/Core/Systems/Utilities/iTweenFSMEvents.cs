using HutongGames.PlayMaker.Actions;
using UnityEngine;

#nullable disable

public class iTweenFSMEvents : MonoBehaviour
  {
    public static int itweenIDCount;
    public int itweenID;
    public iTweenFsmAction itweenFSMAction;
    public bool donotfinish;
    public bool islooping;

    private void iTweenOnStart(int aniTweenID)
    {
      if (this.itweenID != aniTweenID)
        return;
      this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.startEvent);
    }

    private void iTweenOnComplete(int aniTweenID)
    {
      if (this.itweenID != aniTweenID)
        return;
      if (this.islooping)
      {
        if (this.donotfinish)
          return;
        this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.finishEvent);
        this.itweenFSMAction.Finish();
      }
      else
      {
        this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.finishEvent);
        this.itweenFSMAction.Finish();
      }
    }
  }

