// Decompiled with JetBrains decompiler
// Type: UsableBasicWarp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class UsableBasicWarp : BraveBehaviour, IPlayerInteractable
    {
      public bool IsRatTrapdoorLadder;
      public bool IsHelicopterLadder;
      private bool m_justWarped;

      private void Start()
      {
        GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).RegisterInteractable((IPlayerInteractable) this);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        return Vector2.Distance(point, this.sprite.WorldBottomCenter);
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        if (!this.IsHelicopterLadder)
          return;
        this.m_justWarped = false;
      }

      public void OnExitRange(PlayerController interactor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_justWarped)
          return;
        if (!this.IsRatTrapdoorLadder)
          this.StartCoroutine(this.HandleWarpCooldown(interactor));
        else
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleWarpCooldown(interactor));
      }

      [DebuggerHidden]
      private IEnumerator HandleWarpCooldown(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new UsableBasicWarp.<HandleWarpCooldown>c__Iterator0()
        {
          player = player,
          _this = this
        };
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;
    }

}
