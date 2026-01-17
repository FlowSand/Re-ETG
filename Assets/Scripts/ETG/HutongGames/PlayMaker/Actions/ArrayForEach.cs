// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayForEach
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Iterate through the items in an Array and run an FSM on each item. NOTE: The FSM has to Finish before being run on the next item.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayForEach : RunFSMAction
  {
    [HutongGames.PlayMaker.Tooltip("Array to iterate through.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray array;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the item in a variable")]
    [HideTypeFilter]
    [MatchElementType("array")]
    public FsmVar storeItem;
    [ActionSection("Run FSM")]
    public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();
    [HutongGames.PlayMaker.Tooltip("Event to send after iterating through all items in the Array.")]
    public FsmEvent finishEvent;
    private int currentIndex;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.fsmTemplateControl = new FsmTemplateControl();
      this.runFsm = (Fsm) null;
    }

    public override void Awake()
    {
      if (this.array == null || !((Object) this.fsmTemplateControl.fsmTemplate != (Object) null) || !Application.isPlaying)
        return;
      this.runFsm = this.Fsm.CreateSubFsm(this.fsmTemplateControl);
    }

    public override void OnEnter()
    {
      if (this.array == null || this.runFsm == null)
      {
        this.Finish();
      }
      else
      {
        this.currentIndex = 0;
        this.StartFsm();
      }
    }

    public override void OnUpdate()
    {
      this.runFsm.Update();
      if (!this.runFsm.Finished)
        return;
      this.StartNextFsm();
    }

    public override void OnFixedUpdate()
    {
      this.runFsm.LateUpdate();
      if (!this.runFsm.Finished)
        return;
      this.StartNextFsm();
    }

    public override void OnLateUpdate()
    {
      this.runFsm.LateUpdate();
      if (!this.runFsm.Finished)
        return;
      this.StartNextFsm();
    }

    private void StartNextFsm()
    {
      ++this.currentIndex;
      this.StartFsm();
    }

    private void StartFsm()
    {
      for (; this.currentIndex < this.array.Length; ++this.currentIndex)
      {
        this.DoStartFsm();
        if (!this.runFsm.Finished)
          return;
      }
      this.Fsm.Event(this.finishEvent);
      this.Finish();
    }

    private void DoStartFsm()
    {
      this.storeItem.SetValue(this.array.Values[this.currentIndex]);
      this.fsmTemplateControl.UpdateValues();
      this.fsmTemplateControl.ApplyOverrides(this.runFsm);
      this.runFsm.OnEnable();
      if (this.runFsm.Started)
        return;
      this.runFsm.Start();
    }

    protected override void CheckIfFinished()
    {
    }
  }
}
