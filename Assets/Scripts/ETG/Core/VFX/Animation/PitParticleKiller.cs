// Decompiled with JetBrains decompiler
// Type: PitParticleKiller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class PitParticleKiller : MonoBehaviour
  {
    private ParticleSystem.Particle[] m_particleArray;
    private ParticleSystem m_system;
    private Dungeonator.Dungeon m_dungeon;
    private Transform m_transform;

    private void Start()
    {
      this.m_transform = this.transform;
      this.m_dungeon = GameManager.Instance.Dungeon;
      this.m_system = this.GetComponent<ParticleSystem>();
      this.m_particleArray = new ParticleSystem.Particle[this.m_system.maxParticles];
    }

    private bool LocalCellSupportsFalling(Vector3 worldPos)
    {
      IntVector2 intVector2 = worldPos.IntXY(VectorConversions.Floor);
      if (!this.m_dungeon.data.CheckInBounds(intVector2))
        return false;
      CellData cellData = this.m_dungeon.data[intVector2];
      return cellData != null && cellData.type == CellType.PIT && !cellData.fallingPrevented;
    }

    private void LateUpdate()
    {
      int particles = this.m_system.GetParticles(this.m_particleArray);
      for (int index = 0; index < particles; ++index)
      {
        if (this.LocalCellSupportsFalling(this.m_transform.TransformPoint(this.m_particleArray[index].position)))
          this.m_particleArray[index].remainingLifetime = 0.0f;
      }
      this.m_system.SetParticles(this.m_particleArray, particles);
    }
  }

