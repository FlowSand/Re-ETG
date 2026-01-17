// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AddScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.ScriptControl)]
  [HutongGames.PlayMaker.Tooltip("Adds a Script to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Script on exiting the state.")]
  public class AddScript : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to add the script to.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Script to add to the GameObject.")]
    [UIHint(UIHint.ScriptComponent)]
    public FsmString script;
    [HutongGames.PlayMaker.Tooltip("Remove the script from the GameObject when this State is exited.")]
    public FsmBool removeOnExit;
    private Component addedComponent;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.script = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoAddComponent(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
      this.Finish();
    }

    public override void OnExit()
    {
      if (!this.removeOnExit.Value || !((Object) this.addedComponent != (Object) null))
        return;
      Object.Destroy((Object) this.addedComponent);
    }

    private void DoAddComponent(GameObject go)
    {
      this.addedComponent = go.AddComponent(ReflectionUtils.GetGlobalType(this.script.Value));
      if (!((Object) this.addedComponent == (Object) null))
        return;
      this.LogError("Can't add script: " + this.script.Value);
    }
  }
}
