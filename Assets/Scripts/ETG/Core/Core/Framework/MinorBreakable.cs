// Decompiled with JetBrains decompiler
// Type: MinorBreakable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class MinorBreakable : BraveBehaviour, IPlaceConfigurable
    {
      public MinorBreakable.BreakStyle breakStyle;
      [BetterList]
      public ShardCluster[] shardClusters;
      public GameObject stainObject;
      public Vector2 ShardSpawnOffset;
      public bool destroyOnBreak = true;
      public float ForcedDestroyDelay;
      public bool makeParallelOnBreak = true;
      public bool resistsExplosions;
      public bool stopsBullets;
      public bool canSpawnFairy;
      [Header("DropLoots")]
      public bool dropCoins;
      public float amountToRain;
      public float chanceToRain;
      [Header("Explosive?")]
      public bool explodesOnBreak;
      public ExplosionData explosionData;
      [Header("Goops?")]
      public bool goopsOnBreak;
      [ShowInInspectorIf("goopsOnBreak", false)]
      public GoopDefinition goopType;
      [ShowInInspectorIf("goopsOnBreak", false)]
      public float goopRadius = 3f;
      [Header("Particulates")]
      public bool hasParticulates;
      [ShowInInspectorIf("hasParticulates", false)]
      public int MinParticlesOnBurst;
      [ShowInInspectorIf("hasParticulates", false)]
      public int MaxParticlesOnBurst;
      [ShowInInspectorIf("hasParticulates", false)]
      public float ParticleSize = 1f / 16f;
      [ShowInInspectorIf("hasParticulates", false)]
      public float ParticleLifespan = 0.25f;
      [ShowInInspectorIf("hasParticulates", false)]
      public float ParticleMagnitude = 1f;
      [ShowInInspectorIf("hasParticulates", false)]
      public float ParticleMagnitudeVariance = 0.5f;
      [ShowInInspectorIf("hasParticulates", false)]
      public Color ParticleColor;
      [ShowInInspectorIf("hasParticulates", false)]
      public GlobalSparksDoer.EmitRegionStyle EmitStyle = GlobalSparksDoer.EmitRegionStyle.RADIAL;
      [ShowInInspectorIf("hasParticulates", false)]
      public GlobalSparksDoer.SparksType ParticleType;
      [Header("Animation and Audio")]
      [CheckAnimation(null)]
      public string breakAnimName;
      public string breakAnimFrame;
      public string breakAudioEventName;
      public System.Action OnBreak;
      public Action<MinorBreakable> OnBreakContext;
      public float AdditionalSpawnedObjectHeight;
      public Vector2 SpawnedObjectOffsetVector = Vector2.zero;
      [NonSerialized]
      public float heightOffGround = 0.1f;
      [NonSerialized]
      public bool OnlyBreaksOnScreen;
      public GameObject AdditionalVFXObject;
      public bool OnlyBrokenByCode;
      public bool isInvulnerableToGameActors;
      [Header("Unusual Settings")]
      public bool CastleReplacedWithWaterDrum;
      [HideInInspector]
      public bool isImpermeableToGameActors;
      [HideInInspector]
      public bool onlyVulnerableToGunfire;
      public bool OnlyPlayerProjectilesCanBreak;
      [HideInInspector]
      public SurfaceDecorator parentSurface;
      public List<DebrisObject> AdditionalDestabilizedObjects;
      public bool ForceSmallForCollisions;
      public bool IgnoredForPotShotsModifier;
      private bool? m_cachedIsBig;
      private bool m_isBroken;
      private Transform m_transform;
      private tk2dSprite m_sprite;
      private tk2dSpriteAnimator m_spriteAnimator;
      private MinorBreakableGroupManager m_groupManager;
      public bool IsDecorativeOnly;
      private bool m_doneAdditionalDestabilize;
      private OccupiedCells m_occupiedCells;

      public bool IsBroken => this.m_isBroken;

      public bool IsBig
      {
        get
        {
          if (this.ForceSmallForCollisions)
            this.m_cachedIsBig = new bool?(false);
          else if (!this.m_cachedIsBig.HasValue && (bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.PrimaryPixelCollider != null)
          {
            PixelCollider pixelCollider = this.specRigidbody.HitboxPixelCollider ?? this.specRigidbody.PrimaryPixelCollider;
            this.m_cachedIsBig = new bool?(pixelCollider.Dimensions.x > 8 || pixelCollider.Dimensions.y > 8);
          }
          return this.m_cachedIsBig.Value;
        }
      }

      public MinorBreakableGroupManager GroupManager
      {
        get => this.m_groupManager;
        set => this.m_groupManager = value;
      }

      public Vector2 CenterPoint
      {
        get
        {
          if ((bool) (UnityEngine.Object) this.specRigidbody)
            return this.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          return (bool) (UnityEngine.Object) this.sprite ? this.sprite.WorldCenter : this.transform.position.XY();
        }
      }

      private void Awake() => StaticReferenceManager.AllMinorBreakables.Add(this);

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MinorBreakable.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public void CleanupCallbacks()
      {
        if (!(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
      }

      private void OnPreCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherCollider)
      {
        if (myCollider.IsTrigger || this.OnlyBrokenByCode || !this.enabled)
          return;
        if (this.m_isBroken)
        {
          PhysicsEngine.SkipCollision = true;
        }
        else
        {
          if (this.OnlyBreaksOnScreen && !this.renderer.isVisible)
            return;
          Projectile component = otherRigidbody.GetComponent<Projectile>();
          if (this.onlyVulnerableToGunfire && (UnityEngine.Object) component == (UnityEngine.Object) null)
            return;
          if (this.OnlyPlayerProjectilesCanBreak && (bool) (UnityEngine.Object) component && !(component.Owner is PlayerController))
          {
            PhysicsEngine.SkipCollision = true;
          }
          else
          {
            if (this.isInvulnerableToGameActors && (UnityEngine.Object) otherRigidbody.gameActor != (UnityEngine.Object) null)
              return;
            if (this.isImpermeableToGameActors && (UnityEngine.Object) otherRigidbody.gameActor != (UnityEngine.Object) null)
              PhysicsEngine.SkipCollision = true;
            else if (otherRigidbody.gameActor is PlayerController && (otherRigidbody.gameActor as PlayerController).IsEthereal)
            {
              PhysicsEngine.SkipCollision = true;
            }
            else
            {
              if ((UnityEngine.Object) otherRigidbody.minorBreakable != (UnityEngine.Object) null)
                return;
              this.Break(otherRigidbody.Velocity.normalized * Mathf.Min(otherRigidbody.Velocity.magnitude, 5f));
              if (!this.stopsBullets)
                PhysicsEngine.SkipCollision = true;
              if (!((UnityEngine.Object) otherRigidbody.gameActor != (UnityEngine.Object) null))
                return;
              PhysicsEngine.SkipCollision = true;
            }
          }
        }
      }

      private void OnBreakAnimationComplete()
      {
        if (this.explodesOnBreak)
          Exploder.Explode((Vector3) this.specRigidbody.UnitCenter, this.explosionData, Vector2.zero, new System.Action(this.FinishBreak));
        else
          this.FinishBreak();
      }

      private void FinishBreak()
      {
        if (this.goopsOnBreak)
          DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopType).TimedAddGoopCircle(this.specRigidbody.UnitCenter, this.goopRadius);
        if (this.destroyOnBreak)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
        {
          if (!this.makeParallelOnBreak)
            return;
          this.m_sprite.IsPerpendicular = false;
        }
      }

      public void SpawnShards(
        Vector2 direction,
        float minAngle,
        float maxAngle,
        float verticalSpeed,
        float minMagnitude,
        float maxMagnitude)
      {
        if (GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.VERY_LOW)
          return;
        if ((UnityEngine.Object) this.m_sprite == (UnityEngine.Object) null || (UnityEngine.Object) this.m_transform == (UnityEngine.Object) null)
        {
          this.m_transform = this.transform;
          this.m_sprite = this.GetComponent<tk2dSprite>();
        }
        if ((UnityEngine.Object) this.m_sprite == (UnityEngine.Object) null || (UnityEngine.Object) this.m_transform == (UnityEngine.Object) null)
        {
          UnityEngine.Debug.LogError((object) "trying to spawn shards on a object with no transform or sprite");
        }
        else
        {
          Vector3 position = this.m_sprite.WorldCenter.ToVector3ZUp(this.m_sprite.WorldCenter.y) + this.ShardSpawnOffset.ToVector3ZUp();
          if (this.shardClusters != null && this.shardClusters.Length > 0)
          {
            int iterator = UnityEngine.Random.Range(0, 10);
            for (int index1 = 0; index1 < this.shardClusters.Length; ++index1)
            {
              ShardCluster shardCluster = this.shardClusters[index1];
              int num1 = UnityEngine.Random.Range(shardCluster.minFromCluster, shardCluster.maxFromCluster + 1);
              int num2 = UnityEngine.Random.Range(0, shardCluster.clusterObjects.Length);
              for (int index2 = 0; index2 < num1; ++index2)
              {
                float discrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(iterator);
                ++iterator;
                Vector3 a = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(minAngle, maxAngle, discrepancyRandom)) * (direction.normalized * UnityEngine.Random.Range(minMagnitude, maxMagnitude)).ToVector3ZUp(verticalSpeed);
                int index3 = (num2 + index2) % shardCluster.clusterObjects.Length;
                GameObject gameObject = SpawnManager.SpawnDebris(shardCluster.clusterObjects[index3].gameObject, position, Quaternion.identity);
                tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                if ((UnityEngine.Object) this.m_sprite.attachParent != (UnityEngine.Object) null && (UnityEngine.Object) component != (UnityEngine.Object) null)
                {
                  component.attachParent = this.m_sprite.attachParent;
                  component.HeightOffGround = this.m_sprite.HeightOffGround;
                }
                gameObject.GetComponent<DebrisObject>().Trigger(Vector3.Scale(a, shardCluster.forceAxialMultiplier) * shardCluster.forceMultiplier, this.heightOffGround + this.AdditionalSpawnedObjectHeight, shardCluster.rotationMultiplier);
              }
            }
          }
          if (!((UnityEngine.Object) this.AdditionalVFXObject != (UnityEngine.Object) null))
            return;
          SpawnManager.SpawnVFX(this.AdditionalVFXObject, position, Quaternion.identity);
        }
      }

      private void SpawnStain()
      {
        if (!((UnityEngine.Object) this.stainObject != (UnityEngine.Object) null))
          return;
        GameObject vfx = SpawnManager.SpawnDecal(this.stainObject);
        tk2dSprite component1 = vfx.GetComponent<tk2dSprite>();
        component1.PlaceAtPositionByAnchor(this.sprite.WorldCenter.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
        component1.HeightOffGround = 0.1f;
        if ((UnityEngine.Object) this.parentSurface != (UnityEngine.Object) null && !this.parentSurface.IsDestabilized)
        {
          component1.HeightOffGround = 0.1f;
          if ((UnityEngine.Object) this.parentSurface.sprite != (UnityEngine.Object) null)
          {
            this.parentSurface.sprite.AttachRenderer((tk2dBaseSprite) component1);
            this.parentSurface.sprite.UpdateZDepth();
          }
          MajorBreakable component2 = this.parentSurface.GetComponent<MajorBreakable>();
          if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
            return;
          component2.AttachDestructibleVFX(vfx);
        }
        else
        {
          component1.HeightOffGround = -1f;
          component1.UpdateZDepth();
        }
      }

      private void HandleSynergies()
      {
        if (this.IgnoredForPotShotsModifier || this.OnlyBrokenByCode)
          return;
        int count = 0;
        if (!PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.MINOR_BLANKABLES, out count) || (double) UnityEngine.Random.value >= 0.0099999997764825821 * (double) count)
          return;
        GameManager.Instance.BestActivePlayer.ForceBlank(overrideCenter: new Vector2?(!(bool) (UnityEngine.Object) this.sprite ? this.transform.position.XY() : this.sprite.WorldCenter), breaksObjects: false);
      }

      private void HandleShardSpawns(Vector2 sourceVelocity)
      {
        MinorBreakable.BreakStyle breakStyle = this.breakStyle;
        if (sourceVelocity == Vector2.zero)
          breakStyle = MinorBreakable.BreakStyle.BURST;
        float verticalSpeed = 1.5f;
        this.SpawnLoot();
        switch (breakStyle)
        {
          case MinorBreakable.BreakStyle.CONE:
            this.SpawnShards(sourceVelocity, -45f, 45f, verticalSpeed, sourceVelocity.magnitude * 0.5f, sourceVelocity.magnitude * 1.5f);
            break;
          case MinorBreakable.BreakStyle.BURST:
            this.SpawnShards(Vector2.right, -180f, 180f, verticalSpeed, 1f, 2f);
            break;
          case MinorBreakable.BreakStyle.JET:
            this.SpawnShards(sourceVelocity, -15f, 15f, verticalSpeed, sourceVelocity.magnitude * 0.5f, sourceVelocity.magnitude * 1.5f);
            break;
          case MinorBreakable.BreakStyle.WALL_DOWNWARD_BURST:
            this.SpawnShards(Vector2.down, -30f, 30f, 0.0f, 0.25f, 0.75f);
            break;
        }
        this.SpawnStain();
      }

      public void SpawnLoot()
      {
        if (!this.dropCoins || (double) UnityEngine.Random.value >= (double) this.chanceToRain)
          return;
        Vector3 vector = Vector3.up * 2f;
        DebrisObject orAddComponent = SpawnManager.SpawnDebris(GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings.bronzeCoinPrefab, this.specRigidbody.UnitCenter.ToVector3ZUp(this.transform.position.z), Quaternion.identity).GetOrAddComponent<DebrisObject>();
        orAddComponent.shouldUseSRBMotion = true;
        orAddComponent.angularVelocity = 0.0f;
        orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
        orAddComponent.Trigger(vector.WithZ(4f), 0.05f);
        orAddComponent.canRotate = false;
      }

      private void HandleParticulates(Vector2 vel)
      {
        if (!this.hasParticulates)
          return;
        Vector3 worldBottomLeft = (Vector3) this.sprite.WorldBottomLeft;
        Vector3 worldTopRight = (Vector3) this.sprite.WorldTopRight;
        switch (this.EmitStyle)
        {
          case GlobalSparksDoer.EmitRegionStyle.RANDOM:
            GlobalSparksDoer.DoRandomParticleBurst(UnityEngine.Random.Range(this.MinParticlesOnBurst, this.MaxParticlesOnBurst), worldBottomLeft, worldTopRight, (Vector3) (vel.normalized * this.ParticleMagnitude), 45f, this.ParticleMagnitudeVariance, new float?(this.ParticleSize), new float?(this.ParticleLifespan), new Color?(this.ParticleColor), this.ParticleType);
            break;
          case GlobalSparksDoer.EmitRegionStyle.RADIAL:
            GlobalSparksDoer.DoRadialParticleBurst(UnityEngine.Random.Range(this.MinParticlesOnBurst, this.MaxParticlesOnBurst), worldBottomLeft, worldTopRight, 30f, this.ParticleMagnitude, this.ParticleMagnitudeVariance, new float?(this.ParticleSize), new float?(this.ParticleLifespan), new Color?(this.ParticleColor), this.ParticleType);
            break;
        }
      }

      public void Break()
      {
        if (!(bool) (UnityEngine.Object) this || !this.enabled || this.m_isBroken)
          return;
        this.m_isBroken = true;
        if ((UnityEngine.Object) this.m_groupManager != (UnityEngine.Object) null)
          this.m_groupManager.InformBroken(this, Vector2.zero, this.heightOffGround);
        if (GameManager.Instance.InTutorial && !this.name.Contains("table", true) && !this.name.Contains("red", true))
          GameManager.BroadcastRoomTalkDoerFsmEvent("playerBrokeShit");
        if (this.m_occupiedCells != null)
          this.m_occupiedCells.Clear();
        IPlayerInteractable ixable = this.gameObject.GetInterface<IPlayerInteractable>();
        if (ixable != null)
        {
          RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY());
          if (roomFromPosition.IsRegistered(ixable))
            roomFromPosition.DeregisterInteractable(ixable);
        }
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
          this.specRigidbody.enabled = false;
        bool flag = false;
        if ((UnityEngine.Object) this.m_spriteAnimator != (UnityEngine.Object) null && this.breakAnimName != string.Empty)
        {
          tk2dSpriteAnimationClip clipByName = this.m_spriteAnimator.GetClipByName(this.breakAnimName);
          if (clipByName != null)
          {
            this.m_spriteAnimator.Play(clipByName);
            flag = true;
            this.Invoke("OnBreakAnimationComplete", clipByName.BaseClipLength);
          }
        }
        else if (!string.IsNullOrEmpty(this.breakAnimFrame))
          this.m_sprite.SetSprite(this.breakAnimFrame);
        if (!(bool) (UnityEngine.Object) this.m_transform)
          this.m_transform = this.transform;
        if ((bool) (UnityEngine.Object) this.m_transform)
        {
          int num1 = (int) AkSoundEngine.SetObjectPosition(this.gameObject, this.m_transform.position.x, this.m_transform.position.y, this.m_transform.position.z, this.m_transform.forward.x, this.m_transform.forward.y, this.m_transform.forward.z, this.m_transform.up.x, this.m_transform.up.y, this.m_transform.up.z);
        }
        if (!string.IsNullOrEmpty(this.breakAudioEventName))
        {
          int num2 = (int) AkSoundEngine.PostEvent(this.breakAudioEventName, this.gameObject);
        }
        this.HandleShardSpawns(Vector2.zero);
        this.HandleParticulates(Vector2.zero);
        this.HandleSynergies();
        SurfaceDecorator component = this.GetComponent<SurfaceDecorator>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.Destabilize(Vector2.zero);
        this.DestabilizeAttachedObjects(Vector2.zero);
        if (this.OnBreak != null)
          this.OnBreak();
        if (this.OnBreakContext != null)
          this.OnBreakContext(this);
        if (!this.destroyOnBreak || flag)
          return;
        if ((double) this.ForcedDestroyDelay > 0.0)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, this.ForcedDestroyDelay);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      private void DestabilizeAttachedObjects(Vector2 vec)
      {
        if (this.m_doneAdditionalDestabilize)
          return;
        this.m_doneAdditionalDestabilize = true;
        for (int index = 0; index < this.AdditionalDestabilizedObjects.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.AdditionalDestabilizedObjects[index])
          {
            Vector3 startingForce = UnityEngine.Random.insideUnitCircle.ToVector3ZUp(0.5f) * UnityEngine.Random.Range(2.5f, 4f);
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON)
              startingForce.y = Mathf.Abs(startingForce.y) * -1f;
            this.AdditionalDestabilizedObjects[index].transform.parent = SpawnManager.Instance.Debris;
            this.AdditionalDestabilizedObjects[index].Trigger(startingForce, 0.5f);
          }
        }
      }

      public void Break(Vector2 direction)
      {
        if (!this.enabled || this.m_isBroken)
          return;
        this.m_isBroken = true;
        if ((UnityEngine.Object) this.m_groupManager != (UnityEngine.Object) null)
          this.m_groupManager.InformBroken(this, direction, this.heightOffGround);
        bool flag1 = GameManager.Instance.InTutorial;
        if (GameManager.Instance.PrimaryPlayer.CurrentRoom != null)
          flag1 = flag1 || GameManager.Instance.PrimaryPlayer.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL;
        if (flag1 && !this.name.Contains("table", true) && !this.explodesOnBreak)
          GameManager.BroadcastRoomTalkDoerFsmEvent("playerBrokeShit");
        if (this.m_occupiedCells != null)
          this.m_occupiedCells.Clear();
        IPlayerInteractable ixable = this.gameObject.GetInterface<IPlayerInteractable>();
        if (ixable != null)
        {
          RoomHandler roomHandler = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY()) ?? GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY() + IntVector2.Right);
          if (roomHandler != null && roomHandler.IsRegistered(ixable))
            roomHandler.DeregisterInteractable(ixable);
        }
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
          this.specRigidbody.enabled = false;
        bool flag2 = false;
        if ((UnityEngine.Object) this.m_spriteAnimator != (UnityEngine.Object) null && this.breakAnimName != string.Empty)
        {
          tk2dSpriteAnimationClip clipByName = this.m_spriteAnimator.GetClipByName(this.breakAnimName);
          if (clipByName != null)
          {
            this.m_spriteAnimator.Play(clipByName);
            flag2 = true;
            this.Invoke("OnBreakAnimationComplete", clipByName.BaseClipLength);
          }
        }
        else if (!string.IsNullOrEmpty(this.breakAnimFrame))
          this.m_sprite.SetSprite(this.breakAnimFrame);
        if (!(bool) (UnityEngine.Object) this.m_transform)
          this.m_transform = this.transform;
        if ((bool) (UnityEngine.Object) this.m_transform)
        {
          int num1 = (int) AkSoundEngine.SetObjectPosition(this.gameObject, this.m_transform.position.x, this.m_transform.position.y, this.m_transform.position.z, this.m_transform.forward.x, this.m_transform.forward.y, this.m_transform.forward.z, this.m_transform.up.x, this.m_transform.up.y, this.m_transform.up.z);
        }
        if (!string.IsNullOrEmpty(this.breakAudioEventName))
        {
          int num2 = (int) AkSoundEngine.PostEvent(this.breakAudioEventName, this.gameObject);
        }
        this.HandleShardSpawns(direction);
        this.HandleParticulates(direction);
        this.HandleSynergies();
        SurfaceDecorator component = this.GetComponent<SurfaceDecorator>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.Destabilize(direction.normalized);
        this.DestabilizeAttachedObjects(direction.normalized);
        if (this.canSpawnFairy && GameManager.Instance.Dungeon.sharedSettingsPrefab.RandomShouldSpawnPotFairy())
        {
          IntVector2 intVector2 = this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
          RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector2);
          PotFairyEngageDoer.InstantSpawn = true;
          AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(GameManager.Instance.Dungeon.sharedSettingsPrefab.PotFairyGuid), intVector2, roomFromPosition, true);
        }
        if (this.OnBreak != null)
          this.OnBreak();
        if (this.OnBreakContext != null)
          this.OnBreakContext(this);
        if (!this.destroyOnBreak || flag2)
          return;
        if ((double) this.ForcedDestroyDelay > 0.0)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, this.ForcedDestroyDelay);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      protected override void OnDestroy()
      {
        StaticReferenceManager.AllMinorBreakables.Remove(this);
        base.OnDestroy();
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
        if (!this.isInvulnerableToGameActors || !((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null))
          return;
        this.specRigidbody.Initialize();
        this.m_occupiedCells = new OccupiedCells(this.specRigidbody, room);
      }

      public enum BreakStyle
      {
        CONE = 0,
        BURST = 1,
        JET = 2,
        WALL_DOWNWARD_BURST = 3,
        CUSTOM = 100, // 0x00000064
      }
    }

}
