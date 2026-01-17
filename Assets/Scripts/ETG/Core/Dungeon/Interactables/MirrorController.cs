// Decompiled with JetBrains decompiler
// Type: MirrorController
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
    public class MirrorController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
      public MirrorDweller PlayerReflection;
      public MirrorDweller CoopPlayerReflection;
      public MirrorDweller ChestReflection;
      public tk2dBaseSprite ChestSprite;
      public tk2dBaseSprite MirrorSprite;
      public GameObject ShatterSystem;
      public float CURSE_EXPOSED = 3f;

      private void Start()
      {
        this.PlayerReflection.TargetPlayer = GameManager.Instance.PrimaryPlayer;
        this.PlayerReflection.MirrorSprite = this.MirrorSprite;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        {
          this.CoopPlayerReflection.TargetPlayer = GameManager.Instance.SecondaryPlayer;
          this.CoopPlayerReflection.MirrorSprite = this.MirrorSprite;
        }
        else
          this.CoopPlayerReflection.gameObject.SetActive(false);
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        Chest chest = GameManager.Instance.RewardManager.GenerationSpawnRewardChestAt(this.transform.position.IntXY() + new IntVector2(0, -2) - absoluteRoom.area.basePosition, absoluteRoom);
        chest.PreventFuse = true;
        SpriteOutlineManager.RemoveOutlineFromSprite(chest.sprite);
        Transform transform = chest.gameObject.transform.Find("Shadow");
        if ((bool) (Object) transform)
          chest.ShadowSprite = transform.GetComponent<tk2dSprite>();
        chest.IsMirrorChest = true;
        chest.ConfigureOnPlacement(this.GetAbsoluteParentRoom());
        if ((bool) (Object) chest.majorBreakable)
          chest.majorBreakable.TemporarilyInvulnerable = true;
        this.ChestSprite = chest.sprite;
        this.ChestSprite.renderer.enabled = false;
        this.ChestReflection.TargetSprite = this.ChestSprite;
        this.ChestReflection.MirrorSprite = this.MirrorSprite;
        this.MirrorSprite.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollisionWithMirror);
        MinorBreakable componentInChildren = this.GetComponentInChildren<MinorBreakable>();
        componentInChildren.OnlyBrokenByCode = true;
        componentInChildren.heightOffGround = 4f;
      }

      private void HandleRigidbodyCollisionWithMirror(CollisionData rigidbodyCollision)
      {
        if (!(bool) (Object) rigidbodyCollision.OtherRigidbody.projectile)
          return;
        this.GetAbsoluteParentRoom().DeregisterInteractable((IPlayerInteractable) this);
        if (rigidbodyCollision.OtherRigidbody.projectile.Owner is PlayerController)
          this.StartCoroutine(this.HandleShatter(rigidbodyCollision.OtherRigidbody.projectile.Owner as PlayerController, true));
        else
          this.StartCoroutine(this.HandleShatter(GameManager.Instance.PrimaryPlayer, true));
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        Bounds bounds = this.ChestSprite.GetBounds();
        bounds.SetMinMax(bounds.min + this.ChestSprite.transform.position, bounds.max + this.ChestSprite.transform.position);
        float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
        float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
        return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
      }

      public void OnEnteredRange(PlayerController interactor)
      {
      }

      public void OnExitRange(PlayerController interactor)
      {
        MirrorDweller[] componentsInChildren = this.ChestReflection.GetComponentsInChildren<MirrorDweller>(true);
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if (componentsInChildren[index].UsesOverrideTintColor)
            componentsInChildren[index].renderer.enabled = false;
        }
      }

      public void Interact(PlayerController interactor)
      {
        this.ChestSprite.GetComponent<Chest>().ForceOpen(interactor);
        MirrorDweller[] componentsInChildren = this.ChestReflection.GetComponentsInChildren<MirrorDweller>(true);
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if (componentsInChildren[index].UsesOverrideTintColor)
            componentsInChildren[index].renderer.enabled = false;
        }
        this.GetAbsoluteParentRoom().DeregisterInteractable((IPlayerInteractable) this);
        this.StartCoroutine(this.HandleShatter(interactor));
        int index1 = 0;
        while (index1 < interactor.passiveItems.Count && !(interactor.passiveItems[index1] is YellowChamberItem))
          ++index1;
      }

      [DebuggerHidden]
      private IEnumerator HandleShatter(PlayerController interactor, bool skipInitialWait = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MirrorController.<HandleShatter>c__Iterator0()
        {
          skipInitialWait = skipInitialWait,
          interactor = interactor,
          $this = this
        };
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        room.OptionalDoorTopDecorable = ResourceCache.Acquire("Global Prefabs/Purple_Lantern") as GameObject;
        if (room.IsOnCriticalPath || room.connectedRooms.Count != 1)
          return;
        room.ShouldAttemptProceduralLock = true;
        room.AttemptProceduralLockChance = Mathf.Max(room.AttemptProceduralLockChance, Random.Range(0.3f, 0.5f));
      }
    }

}
