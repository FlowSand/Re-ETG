// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetOwner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GameObject)]
  [Tooltip("Gets the Game Object that owns the FSM and stores it in a game object variable.")]
  public class GetOwner : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeGameObject;

    public override void Reset() => this.storeGameObject = (FsmGameObject) null;

    public override void OnEnter()
    {
      this.storeGameObject.Value = this.Owner;
      this.Finish();
    }
  }
}
