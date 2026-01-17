// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AddComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Adds a Component to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Component on exiting the state.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class AddComponent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to add the Component to.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [Title("Component Type")]
    [HutongGames.PlayMaker.Tooltip("The type of Component to add to the Game Object.")]
    [RequiredField]
    [UIHint(UIHint.ScriptComponent)]
    public FsmString component;
    [UIHint(UIHint.Variable)]
    [ObjectType(typeof (Component))]
    [HutongGames.PlayMaker.Tooltip("Store the component in an Object variable. E.g., to use with Set Property.")]
    public FsmObject storeComponent;
    [HutongGames.PlayMaker.Tooltip("Remove the Component when this State is exited.")]
    public FsmBool removeOnExit;
    private Component addedComponent;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.component = (FsmString) null;
      this.storeComponent = (FsmObject) null;
    }

    public override void OnEnter()
    {
      this.DoAddComponent();
      this.Finish();
    }

    public override void OnExit()
    {
      if (!this.removeOnExit.Value || !((Object) this.addedComponent != (Object) null))
        return;
      Object.Destroy((Object) this.addedComponent);
    }

    private void DoAddComponent()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this.addedComponent = ownerDefaultTarget.AddComponent(ReflectionUtils.GetGlobalType(this.component.Value));
      this.storeComponent.Value = (Object) this.addedComponent;
      if (!((Object) this.addedComponent == (Object) null))
        return;
      this.LogError("Can't add component: " + this.component.Value);
    }
  }
}
