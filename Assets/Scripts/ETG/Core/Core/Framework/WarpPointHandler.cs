// Decompiled with JetBrains decompiler
// Type: WarpPointHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class WarpPointHandler : BraveBehaviour
    {
      [NonSerialized]
      public bool DISABLED_TEMPORARILY;
      public WarpPointHandler.WarpTargetType warpTarget;
      public bool OnlyReceiver;
      public MajorBreakable OptionalCover;
      public Vector2 AdditionalSpawnOffset;
      [NonSerialized]
      public Vector2 spawnOffset = Vector2.zero;
      [NonSerialized]
      public bool ManuallyAssigned;
      public Func<PlayerController, float> OnPreWarp;
      public Func<PlayerController, float> OnWarping;
      public Func<PlayerController, float> OnWarpDone;
      private WarpPointHandler m_targetWarper;
      private static bool m_justWarped;

      public void SetTarget(WarpPointHandler target)
      {
        this.warpTarget = ~WarpPointHandler.WarpTargetType.WARP_A;
        this.ManuallyAssigned = true;
        this.m_targetWarper = target;
      }

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WarpPointHandler.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      private void TryAcquirePairedWarp()
      {
        WarpPointHandler[] objectsOfType = UnityEngine.Object.FindObjectsOfType<WarpPointHandler>();
        for (int index = 0; index < objectsOfType.Length; ++index)
        {
          if (objectsOfType[index].warpTarget == this.warpTarget && (UnityEngine.Object) this != (UnityEngine.Object) objectsOfType[index])
          {
            this.m_targetWarper = objectsOfType[index];
            break;
          }
        }
      }

      public Vector2 GetTargetPoint()
      {
        return (!this.ManuallyAssigned ? this.m_targetWarper.specRigidbody.UnitCenter : this.m_targetWarper.specRigidbody.UnitBottomCenter) + new Vector2(-0.5f, !this.ManuallyAssigned ? 0.0f : -0.125f) + this.spawnOffset + this.m_targetWarper.AdditionalSpawnOffset;
      }

      private void HandleTriggerEntered(
        SpeculativeRigidbody specRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (WarpPointHandler.m_justWarped || this.OnlyReceiver || this.DISABLED_TEMPORARILY)
          return;
        PlayerController component = specRigidbody.GetComponent<PlayerController>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
          return;
        if ((UnityEngine.Object) this.m_targetWarper == (UnityEngine.Object) null)
          this.TryAcquirePairedWarp();
        if ((UnityEngine.Object) this.m_targetWarper == (UnityEngine.Object) null)
          return;
        Pixelator.Instance.StartCoroutine(this.HandleWarpCooldown(component));
      }

      [DebuggerHidden]
      private IEnumerator HandleWarpCooldown(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WarpPointHandler.<HandleWarpCooldown>c__Iterator1()
        {
          player = player,
          _this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();

      public enum WarpTargetType
      {
        WARP_A,
        WARP_B,
        WARP_C,
        WARP_D,
        WARP_E,
        WARP_F,
        WARP_G,
        WARP_H,
        WARP_I,
      }
    }

}
