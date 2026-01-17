// Decompiled with JetBrains decompiler
// Type: EmergencyCrateController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class EmergencyCrateController : BraveBehaviour
    {
      public string driftAnimationName;
      public string driftSucksAnimationName;
      public string landedAnimationName;
      public string landedSucksAnimationName;
      public string chuteLandedAnimationName;
      public string crateDisappearAnimationName;
      public string shittyCrateDisappearAnimationName;
      public tk2dSpriteAnimator chuteAnimator;
      public GameObject landingTargetSprite;
      public bool usesLootData;
      public LootData lootData;
      public GenericLootTable gunTable;
      public float ChanceToSpawnEnemy;
      public DungeonPlaceable EnemyPlaceable;
      public float ChanceToExplode;
      public ExplosionData ExplosionData;
      private bool m_hasBeenTriggered;
      private Vector3 m_currentPosition;
      private Vector3 m_currentVelocity;
      private RoomHandler m_parentRoom;
      private bool m_crateSucks;
      private GameObject m_landingTarget;

      public void Trigger(
        Vector3 startingVelocity,
        Vector3 startingPosition,
        RoomHandler room,
        bool crateSucks = true)
      {
        this.m_parentRoom = room;
        this.m_currentPosition = startingPosition;
        this.m_currentVelocity = startingVelocity;
        this.m_hasBeenTriggered = true;
        this.spriteAnimator.Play(!crateSucks ? this.driftAnimationName : this.driftSucksAnimationName);
        this.m_crateSucks = crateSucks;
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
        float num = startingPosition.z / -startingVelocity.z;
        this.m_landingTarget = SpawnManager.SpawnVFX(this.landingTargetSprite, startingPosition + num * startingVelocity, Quaternion.identity);
        this.m_landingTarget.GetComponentInChildren<tk2dSprite>().UpdateZDepth();
      }

      private void Update()
      {
        if (!this.m_hasBeenTriggered)
          return;
        this.m_currentPosition += this.m_currentVelocity * BraveTime.DeltaTime;
        if ((double) this.m_currentPosition.z <= 0.0)
        {
          this.m_currentPosition.z = 0.0f;
          this.OnLanded();
        }
        this.transform.position = BraveUtility.QuantizeVector(this.m_currentPosition.WithZ(this.m_currentPosition.y - this.m_currentPosition.z), (float) PhysicsEngine.Instance.PixelsPerUnit);
        this.sprite.HeightOffGround = this.m_currentPosition.z;
        this.sprite.UpdateZDepth();
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnLanded()
      {
        this.m_hasBeenTriggered = false;
        this.sprite.gameObject.layer = LayerMask.NameToLayer("FG_Critical");
        this.sprite.renderer.sortingLayerName = "Background";
        this.sprite.IsPerpendicular = false;
        this.sprite.HeightOffGround = -1f;
        this.m_currentPosition.z = -1f;
        this.spriteAnimator.Play(!this.m_crateSucks ? this.landedAnimationName : this.landedSucksAnimationName);
        this.chuteAnimator.PlayAndDestroyObject(this.chuteLandedAnimationName);
        if ((bool) (Object) this.m_landingTarget)
          SpawnManager.Despawn(this.m_landingTarget);
        this.m_landingTarget = (GameObject) null;
        if ((double) Random.value < (double) this.ChanceToExplode)
        {
          Exploder.Explode((Vector3) this.sprite.WorldCenter, this.ExplosionData, Vector2.zero, ignoreQueues: true);
          this.StartCoroutine(this.DestroyCrateDelayed());
        }
        else if ((double) Random.value < (double) this.ChanceToSpawnEnemy)
        {
          DungeonPlaceableVariant placeableVariant = this.EnemyPlaceable.SelectFromTiersFull();
          if (placeableVariant != null && (Object) placeableVariant.GetOrLoadPlaceableObject != (Object) null)
            AIActor.Spawn(placeableVariant.GetOrLoadPlaceableObject.GetComponent<AIActor>(), this.sprite.WorldCenter.ToIntVector2(), GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.sprite.WorldCenter.ToIntVector2()), true);
          this.StartCoroutine(this.DestroyCrateDelayed());
        }
        else
        {
          GameObject original = !this.usesLootData ? this.gunTable.SelectByWeight() : this.lootData.GetItemsForPlayer(GameManager.Instance.PrimaryPlayer)[0].gameObject;
          if ((Object) original.GetComponent<AmmoPickup>() != (Object) null)
            this.StartCoroutine(this.DestroyCrateWhenPickedUp(LootEngine.SpawnItem(original, this.sprite.WorldCenter.ToVector3ZUp() + new Vector3(-0.5f, 0.5f, 0.0f), Vector2.zero, 0.0f, false).GetComponent<AmmoPickup>()));
          else if ((Object) original.GetComponent<Gun>() != (Object) null)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(original);
            gameObject.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.sprite.WorldCenter.ToVector3ZUp() + new Vector3(0.0f, 0.5f, 0.0f), tk2dBaseSprite.Anchor.MiddleCenter);
            Gun component = gameObject.GetComponent<Gun>();
            component.Initialize((GameActor) null);
            component.DropGun();
            this.StartCoroutine(this.DestroyCrateWhenPickedUp(component));
          }
          else
            this.StartCoroutine(this.DestroyCrateWhenPickedUp(LootEngine.SpawnItem(original, this.sprite.WorldCenter.ToVector3ZUp() + new Vector3(-0.5f, 0.5f, 0.0f), Vector2.zero, 0.0f, false)));
        }
      }

      [DebuggerHidden]
      private IEnumerator DestroyCrateDelayed()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EmergencyCrateController.\u003CDestroyCrateDelayed\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator DestroyCrateWhenPickedUp(DebrisObject spawned)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EmergencyCrateController.\u003CDestroyCrateWhenPickedUp\u003Ec__Iterator1()
        {
          spawned = spawned,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator DestroyCrateWhenPickedUp(AmmoPickup spawnedAmmo)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EmergencyCrateController.\u003CDestroyCrateWhenPickedUp\u003Ec__Iterator2()
        {
          spawnedAmmo = spawnedAmmo,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator DestroyCrateWhenPickedUp(Gun spawnedGun)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EmergencyCrateController.\u003CDestroyCrateWhenPickedUp\u003Ec__Iterator3()
        {
          spawnedGun = spawnedGun,
          \u0024this = this
        };
      }

      public void ClearLandingTarget()
      {
        if ((bool) (Object) this.m_landingTarget)
          SpawnManager.Despawn(this.m_landingTarget);
        this.m_landingTarget = (GameObject) null;
      }
    }

}
