// Decompiled with JetBrains decompiler
// Type: InfinilichNegativeSpacePie1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Infinilich/NegativeSpacePie1")]
    public class InfinilichNegativeSpacePie1 : Script
    {
      private const int NumRays = 56;
      private const float SafeDegrees = 45f;
      private const int NumBullets = 14;
      private const float RayLength = 22f;
      private const int SetupTime = 80 /*0x50*/;
      private const float SpinSpeed = 0.42f;
      private const int NumTransitions = 6;
      private const int MidTransitionTime = 35;
      private const int TransitionTellTime = 30;
      private const int ForwardCount = 4;
      private const int ForwardTransitionTime = 150;
      private const int BackwardCount = 1;
      private const int BackwardTransitionTime = 90;
      private static float DeltaRay = 5.72727251f;
      private static float SafeEndAngle;
      private PooledLinkedList<InfinilichNegativeSpacePie1.RayBullet> m_leadBullets = new PooledLinkedList<InfinilichNegativeSpacePie1.RayBullet>();
      private bool m_done;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InfinilichNegativeSpacePie1__Topc__Iterator0()
        {
          _this = this
        };
      }

      private InfinilichNegativeSpacePie1.RayBullet SpawnRay(float angle, int spawnDelay)
      {
        InfinilichNegativeSpacePie1.RayBullet leadBullet = (InfinilichNegativeSpacePie1.RayBullet) null;
        for (int index = 0; index < 14; ++index)
        {
          InfinilichNegativeSpacePie1.RayBullet rayBullet = new InfinilichNegativeSpacePie1.RayBullet(leadBullet, angle, spawnDelay, this.Position);
          if (leadBullet == null)
            leadBullet = rayBullet;
          this.Fire(new Offset(Mathf.Lerp(1.5f, 22f, (float) index / 13f), rotation: angle, transform: string.Empty), new Brave.BulletScript.Speed(), (Bullet) rayBullet);
        }
        return leadBullet;
      }

      [DebuggerHidden]
      private IEnumerator HandleGaps()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InfinilichNegativeSpacePie1__HandleGapsc__Iterator1()
        {
          _this = this
        };
      }

      public class RayBullet : Bullet
      {
        public bool ShouldDestroy;
        public int DestroyDirection;
        public float Angle;
        private InfinilichNegativeSpacePie1.RayBullet m_leadBullet;
        private int m_spawnDelay;
        private Vector2 m_origin;
        private bool m_doTell;

        public RayBullet(
          InfinilichNegativeSpacePie1.RayBullet leadBullet,
          float angle,
          int spawnDelay,
          Vector2 origin)
          : base("pieBullet")
        {
          this.m_leadBullet = leadBullet;
          this.Angle = angle;
          this.m_spawnDelay = spawnDelay;
          this.m_origin = origin;
        }

        public bool DoTell
        {
          get => this.m_doTell;
          set
          {
            if (this.m_doTell == value)
              return;
            if (value)
              this.Projectile.spriteAnimator.Play();
            else
              this.Projectile.spriteAnimator.StopAndResetFrameToDefault();
            this.m_doTell = value;
          }
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new InfinilichNegativeSpacePie1.RayBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
