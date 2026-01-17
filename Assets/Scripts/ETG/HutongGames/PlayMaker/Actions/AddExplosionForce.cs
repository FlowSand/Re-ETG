// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AddExplosionForce
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Applies a force to a Game Object that simulates explosion effects. The explosion force will fall off linearly with distance. Hint: Use the Explosion Action instead to apply an explosion force to all objects in a blast radius.")]
  [ActionCategory(ActionCategory.Physics)]
  public class AddExplosionForce : ComponentAction<Rigidbody>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to add the explosion force to.")]
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The center of the explosion. Hint: this is often the position returned from a GetCollisionInfo action.")]
    public FsmVector3 center;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The strength of the explosion.")]
    public FsmFloat force;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The radius of the explosion. Force falls off linearly with distance.")]
    public FsmFloat radius;
    [HutongGames.PlayMaker.Tooltip("Applies the force as if it was applied from beneath the object. This is useful since explosions that throw things up instead of pushing things to the side look cooler. A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.")]
    public FsmFloat upwardsModifier;
    [HutongGames.PlayMaker.Tooltip("The type of force to apply. See Unity Physics docs.")]
    public ForceMode forceMode;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.center = fsmVector3;
      this.upwardsModifier = (FsmFloat) 0.0f;
      this.forceMode = ForceMode.Force;
      this.everyFrame = false;
    }

    public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

    public override void OnEnter()
    {
      this.DoAddExplosionForce();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnFixedUpdate() => this.DoAddExplosionForce();

    private void DoAddExplosionForce()
    {
      GameObject go = this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner;
      if (this.center == null || !this.UpdateCache(go))
        return;
      this.rigidbody.AddExplosionForce(this.force.Value, this.center.Value, this.radius.Value, this.upwardsModifier.Value, this.forceMode);
    }
  }
}
