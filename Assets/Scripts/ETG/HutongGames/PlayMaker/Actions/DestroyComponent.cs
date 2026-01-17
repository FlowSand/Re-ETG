// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DestroyComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Destroys a Component of an Object.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class DestroyComponent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the Component.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The name of the Component to destroy.")]
    [RequiredField]
    [UIHint(UIHint.ScriptComponent)]
    public FsmString component;
    private Component aComponent;

    public override void Reset()
    {
      this.aComponent = (Component) null;
      this.gameObject = (FsmOwnerDefault) null;
      this.component = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoDestroyComponent(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
      this.Finish();
    }

    private void DoDestroyComponent(GameObject go)
    {
      this.aComponent = go.GetComponent(ReflectionUtils.GetGlobalType(this.component.Value));
      if ((Object) this.aComponent == (Object) null)
        this.LogError("No such component: " + this.component.Value);
      else
        Object.Destroy((Object) this.aComponent);
    }
  }
}
