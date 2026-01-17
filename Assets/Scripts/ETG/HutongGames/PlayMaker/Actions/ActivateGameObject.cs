// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ActivateGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GameObject)]
  [HutongGames.PlayMaker.Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
  public class ActivateGameObject : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to activate/deactivate.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Check to activate, uncheck to deactivate Game Object.")]
    public FsmBool activate;
    [HutongGames.PlayMaker.Tooltip("Recursively activate/deactivate all children.")]
    public FsmBool recursive;
    [HutongGames.PlayMaker.Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
    public bool resetOnExit;
    [HutongGames.PlayMaker.Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
    public bool everyFrame;
    private GameObject activatedGameObject;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.activate = (FsmBool) true;
      this.recursive = (FsmBool) true;
      this.resetOnExit = false;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoActivateGameObject();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoActivateGameObject();

    public override void OnExit()
    {
      if ((Object) this.activatedGameObject == (Object) null || !this.resetOnExit)
        return;
      if (this.recursive.Value)
        this.SetActiveRecursively(this.activatedGameObject, !this.activate.Value);
      else
        this.activatedGameObject.SetActive(!this.activate.Value);
    }

    private void DoActivateGameObject()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if (this.recursive.Value)
        this.SetActiveRecursively(ownerDefaultTarget, this.activate.Value);
      else
        ownerDefaultTarget.SetActive(this.activate.Value);
      this.activatedGameObject = ownerDefaultTarget;
    }

    public void SetActiveRecursively(GameObject go, bool state)
    {
      go.SetActive(state);
      foreach (Component component in go.transform)
        this.SetActiveRecursively(component.gameObject, state);
    }
  }
}
