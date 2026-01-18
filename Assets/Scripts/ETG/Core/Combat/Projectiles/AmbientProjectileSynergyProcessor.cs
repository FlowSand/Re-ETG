// Decompiled with JetBrains decompiler
// Type: AmbientProjectileSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class AmbientProjectileSynergyProcessor : MonoBehaviour
  {
    [LongNumericEnum]
    public CustomSynergyType SynergyToCheck;
    public float TimeBetweenAmbientProjectiles = 5f;
    public bool ActiveEvenWithoutEnemies;
    public bool UsesRadius;
    public float Radius = 5f;
    public RadialBurstInterface Ambience;
    private Gun m_gun;
    private float m_elapsed;

    private void Awake() => this.m_gun = this.GetComponent<Gun>();

    private void Update()
    {
      if (!(bool) (Object) this.m_gun || !(this.m_gun.CurrentOwner is PlayerController))
        return;
      PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
      if (!currentOwner.HasActiveBonusSynergy(this.SynergyToCheck) || !this.ActiveEvenWithoutEnemies && (currentOwner.CurrentRoom == null || !currentOwner.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear)))
        return;
      this.m_elapsed += BraveTime.DeltaTime;
      if (this.UsesRadius)
      {
        float nearestDistance = float.MaxValue;
        currentOwner.CurrentRoom.GetNearestEnemy(currentOwner.CenterPosition, out nearestDistance);
        if ((double) nearestDistance > (double) this.Radius)
          return;
      }
      if ((double) this.m_elapsed <= (double) this.TimeBetweenAmbientProjectiles)
        return;
      this.m_elapsed = 0.0f;
      this.Ambience.DoBurst(currentOwner);
    }
  }

