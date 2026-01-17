// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetParticleCollisionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [Tooltip("Gets info on the last particle collision event. See Unity Particle System docs.")]
  public class GetParticleCollisionInfo : FsmStateAction
  {
    [Tooltip("Get the GameObject hit.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject gameObjectHit;

    public override void Reset() => this.gameObjectHit = (FsmGameObject) null;

    private void StoreCollisionInfo() => this.gameObjectHit.Value = this.Fsm.ParticleCollisionGO;

    public override void OnEnter()
    {
      this.StoreCollisionInfo();
      this.Finish();
    }
  }
}
