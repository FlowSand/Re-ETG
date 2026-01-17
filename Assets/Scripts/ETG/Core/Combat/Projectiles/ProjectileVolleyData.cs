// Decompiled with JetBrains decompiler
// Type: ProjectileVolleyData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class ProjectileVolleyData : ScriptableObject
    {
      public List<ProjectileModule> projectiles;
      public bool UsesBeamRotationLimiter;
      public float BeamRotationDegreesPerSecond = 30f;
      public bool ModulesAreTiers;
      public bool UsesShotgunStyleVelocityRandomizer;
      public float DecreaseFinalSpeedPercentMin = -5f;
      public float IncreaseFinalSpeedPercentMax = 5f;

      public float GetVolleySpeedMod()
      {
        return this.UsesShotgunStyleVelocityRandomizer ? (float) (1.0 + (double) Random.Range(this.DecreaseFinalSpeedPercentMin, this.IncreaseFinalSpeedPercentMax) / 100.0) : 1f;
      }

      public void InitializeFrom(ProjectileVolleyData source)
      {
        this.projectiles = new List<ProjectileModule>();
        this.UsesShotgunStyleVelocityRandomizer = source.UsesShotgunStyleVelocityRandomizer;
        this.DecreaseFinalSpeedPercentMin = source.DecreaseFinalSpeedPercentMin;
        this.IncreaseFinalSpeedPercentMax = source.IncreaseFinalSpeedPercentMax;
        for (int index = 0; index < source.projectiles.Count; ++index)
          this.projectiles.Add(ProjectileModule.CreateClone(source.projectiles[index]));
        this.UsesBeamRotationLimiter = source.UsesBeamRotationLimiter;
        this.BeamRotationDegreesPerSecond = source.BeamRotationDegreesPerSecond;
        this.ModulesAreTiers = source.ModulesAreTiers;
      }
    }

}
