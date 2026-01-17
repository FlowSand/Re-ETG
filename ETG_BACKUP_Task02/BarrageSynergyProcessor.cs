// Decompiled with JetBrains decompiler
// Type: BarrageSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BarrageSynergyProcessor : MonoBehaviour
{
  [LongNumericEnum]
  public CustomSynergyType RequiredSynergy;
  public BarrageModule Barrage;
  public bool BarrageIsAmbient;
  public float MinBarrageCooldown = 5f;
  public float MaxBarrageCooldown = 5f;
  private Gun m_gun;
  private float m_elapsed;
  private float m_currentCooldown;

  private void Start()
  {
    this.m_gun = this.GetComponent<Gun>();
    this.m_currentCooldown = Random.Range(this.MinBarrageCooldown, this.MaxBarrageCooldown);
  }

  private void Update()
  {
    if (Dungeon.IsGenerating || GameManager.IsBossIntro || !this.BarrageIsAmbient || !(bool) (Object) this.m_gun || !(this.m_gun.CurrentOwner is PlayerController))
      return;
    PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
    if (!currentOwner.HasActiveBonusSynergy(this.RequiredSynergy) || !currentOwner.IsInCombat)
      return;
    this.m_elapsed += BraveTime.DeltaTime;
    if ((double) this.m_elapsed < (double) this.m_currentCooldown)
      return;
    this.m_elapsed -= this.m_currentCooldown;
    this.m_currentCooldown = Random.Range(this.MinBarrageCooldown, this.MaxBarrageCooldown);
    this.DoAmbientTargetedBarrage(currentOwner);
  }

  private void DoAmbientTargetedBarrage(PlayerController p)
  {
    List<AIActor> activeEnemies = p.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    if (activeEnemies == null)
      return;
    int index = Random.Range(0, activeEnemies.Count);
    Vector2 normalized = Random.insideUnitCircle.normalized;
    Vector2 startPoint = activeEnemies[index].CenterPosition + -normalized * (this.Barrage.BarrageLength / 2f);
    if (!(bool) (Object) activeEnemies[index])
      return;
    this.Barrage.DoBarrage(startPoint, normalized, (MonoBehaviour) GameManager.Instance.Dungeon);
  }
}
