// Decompiled with JetBrains decompiler
// Type: RadialCharmItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RadialCharmItem : AffectEnemiesInRadiusItem
{
  public bool DoCharm = true;
  [ShowInInspectorIf("DoCharm", false)]
  public GameActorCharmEffect CharmEffect;
  public bool HasProjectileSynergy;
  [LongNumericEnum]
  public CustomSynergyType ProjectileSynergyRequired;
  public ProjectileVolleyData SynergyVolley;

  protected override void AffectEnemy(AIActor target)
  {
    if (!this.DoCharm)
      return;
    target.ApplyEffect((GameActorEffect) this.CharmEffect);
    if (!this.HasProjectileSynergy || !(bool) (Object) this.LastOwner || !this.LastOwner.HasActiveBonusSynergy(this.ProjectileSynergyRequired))
      return;
    VolleyUtility.FireVolley(this.SynergyVolley, this.LastOwner.CenterPosition, target.CenterPosition - this.LastOwner.CenterPosition, (GameActor) this.LastOwner);
  }

  protected override void AffectShop(BaseShopController target)
  {
    if (!this.DoCharm)
      return;
    target.GetComponentInChildren<FakeGameActorEffectHandler>().ApplyEffect((GameActorEffect) this.CharmEffect);
    target.SetCapableOfBeingStolenFrom(true, nameof (RadialCharmItem), new float?(this.CharmEffect.duration));
  }

  protected override void OnDestroy() => base.OnDestroy();
}
