// Decompiled with JetBrains decompiler
// Type: MetalGearRatSidePound1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public abstract class MetalGearRatSidePound1 : Script
    {
      private const float NumWaves = 7f;
      private const int NumBullets = 9;
      private const float EllipseA = 2.5f;
      private const float EllipseB = 1f;
      private const float VerticalDriftVelocity = 0.5f;
      private const float WaftXPeriod = 3f;
      private const float WaftXMagnitude = 0.65f;
      private const float WaftYPeriod = 1f;
      private const float WaftYMagnitude = 0.25f;
      private const int WaftLifeTime = 300;

      protected abstract float StartAngle { get; }

      protected abstract float SweepAngle { get; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatSidePound1__Topc__Iterator0()
        {
          _this = this
        };
      }

      public class WaftBullet : Bullet
      {
        public WaftBullet()
          : base("default_noramp")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MetalGearRatSidePound1.WaftBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
