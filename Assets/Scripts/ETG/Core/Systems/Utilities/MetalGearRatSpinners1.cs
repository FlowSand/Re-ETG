// Decompiled with JetBrains decompiler
// Type: MetalGearRatSpinners1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/MetalGearRat/Spinners1")]
    public class MetalGearRatSpinners1 : Script
    {
      private const int BulletsPerCircle = 21;
      private const float CircleRadius = 5f;
      private const int NearTimeForAttack = 100;
      private List<MetalGearRatSpinners1.CircleDummy> m_circleDummies = new List<MetalGearRatSpinners1.CircleDummy>();

      private Vector2 CenterPoint { get; set; }

      private bool Done { get; set; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatSpinners1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator SpawnInnerRing()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatSpinners1.\u003CSpawnInnerRing\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator SpawnOuterRing()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatSpinners1.\u003CSpawnOuterRing\u003Ec__Iterator2()
        {
          \u0024this = this
        };
      }

      private void SpawnCircle(
        float spawnRadius,
        float spawnAngle,
        float orbitSpeed,
        float rotationSpeed,
        float? initialRadiusBoost = null)
      {
        float magnitude = spawnRadius + (!initialRadiusBoost.HasValue ? 0.0f : initialRadiusBoost.Value);
        Vector2 vector2 = this.CenterPoint + BraveMathCollege.DegreesToVector(spawnAngle, magnitude);
        MetalGearRatSpinners1.CircleDummy circleDummy = new MetalGearRatSpinners1.CircleDummy(this, this.CenterPoint, spawnRadius, spawnAngle, orbitSpeed, initialRadiusBoost);
        circleDummy.Position = vector2;
        circleDummy.Direction = this.AimDirection;
        circleDummy.BulletManager = this.BulletManager;
        circleDummy.Initialize();
        this.m_circleDummies.Add(circleDummy);
        for (int index = 0; index < 21; ++index)
        {
          float angle = this.SubdivideCircle(0.0f, 21, index);
          Vector2 overridePosition = vector2 + BraveMathCollege.DegreesToVector(angle, 5f);
          this.Fire(Offset.OverridePosition(overridePosition), (Bullet) new MetalGearRatSpinners1.CircleBullet(this, circleDummy, rotationSpeed, index, overridePosition - vector2));
        }
      }

      private void SpawnCircleDebris(
        float spawnRadius,
        float spawnAngle,
        float orbitSpeed,
        float tRadius,
        float deltaAngle,
        float? initialRadiusBoost = null)
      {
        float angle = spawnAngle + deltaAngle;
        float radius = Mathf.LerpUnclamped(spawnRadius - 5f, spawnRadius + 5f, tRadius);
        this.Fire(Offset.OverridePosition(this.CenterPoint + BraveMathCollege.DegreesToVector(angle, radius + (!initialRadiusBoost.HasValue ? 0.0f : initialRadiusBoost.Value))), (Bullet) new MetalGearRatSpinners1.OrbitBullet(this, radius, angle, orbitSpeed, initialRadiusBoost));
      }

      public class CircleDummy : Bullet
      {
        public int NearTime;
        public int FireTick = -1;
        private MetalGearRatSpinners1 m_parent;
        private MetalGearRatSpinners1.CircleDummy m_circleDummy;
        private Vector2 m_centerPoint;
        private float m_centerRadius;
        private float m_centerAngle;
        private float m_orbitSpeed;
        private float? m_initialRadiusBoost;

        public CircleDummy(
          MetalGearRatSpinners1 parent,
          Vector2 centerPoint,
          float centerRadius,
          float centerAngle,
          float orbitSpeed,
          float? initialRadiusBoostBoost = null)
          : base("spinner")
        {
          this.m_parent = parent;
          this.m_centerPoint = centerPoint;
          this.m_centerRadius = centerRadius;
          this.m_centerAngle = centerAngle;
          this.m_orbitSpeed = orbitSpeed;
          this.m_initialRadiusBoost = initialRadiusBoostBoost;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatSpinners1.CircleDummy.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      public class CircleBullet : Bullet
      {
        private MetalGearRatSpinners1 m_parent;
        private MetalGearRatSpinners1.CircleDummy m_circleDummy;
        private float m_rotationSpeed;
        private int m_index;
        private Vector2 m_offset;

        public CircleBullet(
          MetalGearRatSpinners1 parent,
          MetalGearRatSpinners1.CircleDummy circleDummy,
          float rotationSpeed,
          int index,
          Vector2 offset)
          : base("spinner")
        {
          this.m_parent = parent;
          this.m_circleDummy = circleDummy;
          this.m_rotationSpeed = rotationSpeed;
          this.m_index = index;
          this.m_offset = offset;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatSpinners1.CircleBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      public class OrbitBullet : Bullet
      {
        private MetalGearRatSpinners1 m_parent;
        private Vector2 m_centerPoint;
        private float m_radius;
        private float m_angle;
        private float m_orbitSpeed;
        private float? m_initialRadiusBoost;

        public OrbitBullet(
          MetalGearRatSpinners1 parent,
          float radius,
          float angle,
          float orbitSpeed,
          float? initialRadiusBoost = null)
          : base("spinner")
        {
          this.m_parent = parent;
          this.m_radius = radius;
          this.m_angle = angle;
          this.m_orbitSpeed = orbitSpeed;
          this.m_initialRadiusBoost = initialRadiusBoost;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatSpinners1.OrbitBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
