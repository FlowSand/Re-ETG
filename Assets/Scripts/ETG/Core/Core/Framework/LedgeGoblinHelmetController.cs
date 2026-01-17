// Decompiled with JetBrains decompiler
// Type: LedgeGoblinHelmetController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class LedgeGoblinHelmetController : BraveBehaviour, IPlayerInteractable
    {
      private bool m_pickedUp;

      private void Start() => SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);

      public float GetDistanceToPoint(Vector2 point)
      {
        Bounds bounds = this.sprite.GetBounds();
        bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
        float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
        float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
        return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        GameStatsManager.Instance.SetFlag(GungeonFlags.LEDGEGOBLIN_ACTIVE_IN_FOYER, true);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (Object) this.sprite)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_pickedUp)
          return;
        this.m_pickedUp = true;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        GameManager.BroadcastRoomTalkDoerFsmEvent("modeAnnoyed");
        bool flag1 = GameStatsManager.Instance.GetFlag(GungeonFlags.LEDGEGOBLIN_COMPLETED_FIRST_DUNGEON);
        bool flag2 = GameStatsManager.Instance.GetFlag(GungeonFlags.LEDGEGOBLIN_COMPLETED_SECOND_DUNGEON);
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.LEDGEGOBLIN_COMPLETED_THIRD_DUNGEON) && flag2 && flag1)
          GameStatsManager.Instance.SetFlag(GungeonFlags.LEDGEGOBLIN_TRIGGERED_THIRD_DUNGEON, true);
        else if (!flag2 && flag1)
          GameStatsManager.Instance.SetFlag(GungeonFlags.LEDGEGOBLIN_TRIGGERED_SECOND_DUNGEON, true);
        else if (!flag1)
          GameStatsManager.Instance.SetFlag(GungeonFlags.LEDGEGOBLIN_TRIGGERED_FIRST_DUNGEON, true);
        RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
        interactor.RemoveBrokenInteractable((IPlayerInteractable) this);
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.LEDGEGOBLIN_COMPLETED_THIRD_DUNGEON))
        {
          this.spriteAnimator.Play("helmte_kick_chain");
          this.transform.position.GetAbsoluteRoom().DeregisterInteractable((IPlayerInteractable) this);
          RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
        }
        else
          this.spriteAnimator.PlayAndDestroyObject(string.Empty);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;

      protected override void OnDestroy() => base.OnDestroy();
    }

}
