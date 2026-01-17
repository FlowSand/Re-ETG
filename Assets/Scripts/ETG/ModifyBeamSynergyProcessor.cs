// Decompiled with JetBrains decompiler
// Type: ModifyBeamSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ModifyBeamSynergyProcessor : MonoBehaviour
{
  public CustomSynergyType SynergyToCheck;
  public bool AddsFreezeEffect;
  [ShowInInspectorIf("AddsFreezeEffect", false)]
  public GameActorFreezeEffect FreezeEffect;
  private Projectile m_projectile;
  private BeamController m_beam;

  public void Awake()
  {
    this.m_projectile = this.GetComponent<Projectile>();
    this.m_beam = this.GetComponent<BeamController>();
  }

  public void Start()
  {
    PlayerController owner = this.m_projectile.Owner as PlayerController;
    if (!(bool) (Object) owner || !owner.HasActiveBonusSynergy(this.SynergyToCheck) || !this.AddsFreezeEffect)
      return;
    this.m_projectile.AppliesFreeze = true;
    this.m_projectile.FreezeApplyChance = 1f;
    this.m_projectile.freezeEffect = this.FreezeEffect;
    this.m_projectile.damageTypes |= CoreDamageTypes.Ice;
    this.m_beam.statusEffectChance = 1f;
    this.m_beam.statusEffectAccumulateMultiplier = 1f;
  }
}
