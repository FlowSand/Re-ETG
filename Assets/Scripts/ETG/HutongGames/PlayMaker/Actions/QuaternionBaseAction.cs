// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.QuaternionBaseAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public abstract class QuaternionBaseAction : FsmStateAction
  {
    [Tooltip("Repeat every frame. Useful if any of the values are changing.")]
    public bool everyFrame;
    [Tooltip("Defines how to perform the action when 'every Frame' is enabled.")]
    public QuaternionBaseAction.everyFrameOptions everyFrameOption;

    public override void Awake()
    {
      if (!this.everyFrame || this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
        return;
      this.Fsm.HandleFixedUpdate = true;
    }

    public enum everyFrameOptions
    {
      Update,
      FixedUpdate,
      LateUpdate,
    }
  }
}
