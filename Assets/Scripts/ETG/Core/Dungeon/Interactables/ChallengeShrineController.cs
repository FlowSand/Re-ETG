// Decompiled with JetBrains decompiler
// Type: ChallengeShrineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ChallengeShrineController : 
      DungeonPlaceableBehaviour,
      IPlayerInteractable,
      IPlaceConfigurable
    {
      public string displayTextKey;
      public string acceptOptionKey;
      public string declineOptionKey;
      public Transform talkPoint;
      public GameObject onPlayerVFX;
      public Vector3 playerVFXOffset = Vector3.zero;
      public bool usesCustomChestTable;
      public WeightedGameObjectCollection CustomChestTable;
      public tk2dBaseSprite AlternativeOutlineTarget;
      private int m_useCount;
      private RoomHandler m_parentRoom;
      private GameObject m_instanceMinimapIcon;
      private float m_noEnemySealTime;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_parentRoom = room;
        this.m_parentRoom.PreventStandardRoomReward = true;
        this.RegisterMinimapIcon();
      }

      private void Update()
      {
        if (!this.m_parentRoom.IsSealed || !(bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer || GameManager.Instance.PrimaryPlayer.CurrentRoom == null)
          return;
        if (GameManager.Instance.PrimaryPlayer.CurrentRoom != this.m_parentRoom)
        {
          this.m_parentRoom.npcSealState = RoomHandler.NPCSealState.SealNone;
          this.m_parentRoom.UnsealRoom();
        }
        else if (!this.m_parentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
        {
          this.m_noEnemySealTime += BraveTime.DeltaTime;
          if ((double) this.m_noEnemySealTime > 3.0)
            this.m_parentRoom.TriggerNextReinforcementLayer();
          if ((double) this.m_noEnemySealTime <= 5.0)
            return;
          this.m_parentRoom.npcSealState = RoomHandler.NPCSealState.SealNone;
          this.m_parentRoom.UnsealRoom();
        }
        else
          this.m_noEnemySealTime = 0.0f;
      }

      public void RegisterMinimapIcon()
      {
        this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject) BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon"));
      }

      public void GetRidOfMinimapIcon()
      {
        if (!((UnityEngine.Object) this.m_instanceMinimapIcon != (UnityEngine.Object) null))
          return;
        Minimap.Instance.DeregisterRoomIcon(this.m_parentRoom, this.m_instanceMinimapIcon);
        this.m_instanceMinimapIcon = (GameObject) null;
      }

      private void DoShrineEffect(PlayerController player)
      {
        this.m_parentRoom.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.SHRINE_WAVE_A);
        this.m_parentRoom.npcSealState = RoomHandler.NPCSealState.SealAll;
        this.m_parentRoom.SealRoom();
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          GameManager.Instance.GetOtherPlayer(player).ReuniteWithOtherPlayer(player);
        this.m_parentRoom.OnEnemiesCleared += new System.Action(this.HandleEnemiesClearedA);
        if ((UnityEngine.Object) this.onPlayerVFX != (UnityEngine.Object) null)
          player.PlayEffectOnActor(this.onPlayerVFX, this.playerVFXOffset);
        this.GetRidOfMinimapIcon();
      }

      private void HandleEnemiesClearedA()
      {
        this.m_parentRoom.OnEnemiesCleared -= new System.Action(this.HandleEnemiesClearedA);
        this.m_parentRoom.OnEnemiesCleared += new System.Action(this.HandleEnemiesClearedB);
        if (this.m_parentRoom.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.SHRINE_WAVE_B))
          return;
        this.HandleFinalEnemiesCleared();
      }

      private void HandleEnemiesClearedB()
      {
        this.m_parentRoom.OnEnemiesCleared -= new System.Action(this.HandleEnemiesClearedB);
        if (!this.m_parentRoom.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.SHRINE_WAVE_C))
          this.HandleFinalEnemiesCleared();
        else
          this.m_parentRoom.OnEnemiesCleared += new System.Action(this.HandleFinalEnemiesCleared);
      }

      private void HandleFinalEnemiesCleared()
      {
        this.m_parentRoom.npcSealState = RoomHandler.NPCSealState.SealNone;
        this.m_parentRoom.OnEnemiesCleared -= new System.Action(this.HandleFinalEnemiesCleared);
        Chest chest = GameManager.Instance.RewardManager.SpawnRewardChestAt(this.m_parentRoom.GetBestRewardLocation(new IntVector2(3, 2)));
        if (!(bool) (UnityEngine.Object) chest)
          return;
        chest.ForceUnlock();
        chest.RegisterChestOnMinimap(this.m_parentRoom);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
          return 100f;
        Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions);
        return Vector2.Distance(point, (Vector2) b) / 1.5f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        SpriteOutlineManager.AddOutlineToSprite(this.AlternativeOutlineTarget ?? this.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite(this.AlternativeOutlineTarget ?? this.sprite);
      }

      [DebuggerHidden]
      private IEnumerator HandleShrineConversation(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChallengeShrineController.\u003CHandleShrineConversation\u003Ec__Iterator0()
        {
          interactor = interactor,
          \u0024this = this
        };
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_useCount > 0)
          return;
        ++this.m_useCount;
        this.m_parentRoom.DeregisterInteractable((IPlayerInteractable) this);
        this.StartCoroutine(this.HandleShrineConversation(interactor));
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
