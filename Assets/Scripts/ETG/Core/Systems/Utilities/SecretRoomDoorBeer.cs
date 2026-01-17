// Decompiled with JetBrains decompiler
// Type: SecretRoomDoorBeer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SecretRoomDoorBeer : MonoBehaviour
    {
      public static List<SecretRoomDoorBeer> AllSecretRoomDoors;
      public DungeonDoorSubsidiaryBlocker subsidiaryBlocker;
      public RuntimeExitDefinition exitDef;
      public RoomHandler linkedRoom;
      public SecretRoomManager manager;
      public SecretRoomExitData collider;
      private MajorBreakable m_breakable;
      private tk2dSprite m_breakVfxSprite;
      public List<BreakableChunk> wallChunks;
      private bool m_hasSnitchedBricked;
      private GameObject m_snitchBrick;
      public BreakFrame[] m_breakFrames;
      private bool m_hasBeenAmygdalaed;
      private float m_amygdalaCheckTimer;
      private GameObject m_amygdala;

      private void Awake()
      {
        if (SecretRoomDoorBeer.AllSecretRoomDoors == null)
          SecretRoomDoorBeer.AllSecretRoomDoors = new List<SecretRoomDoorBeer>();
        SecretRoomDoorBeer.AllSecretRoomDoors.Add(this);
      }

      private void Start()
      {
        if (this.linkedRoom == null)
          return;
        this.linkedRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.HandlePlayerEnteredLinkedRoom);
      }

      private void Update()
      {
        if (this.m_hasBeenAmygdalaed)
          return;
        this.m_amygdalaCheckTimer -= BraveTime.DeltaTime;
        if ((double) this.m_amygdalaCheckTimer >= 0.0)
          return;
        this.m_amygdalaCheckTimer = 1f;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (GameManager.Instance.AllPlayers[index].HasActiveBonusSynergy(CustomSynergyType.INSIGHT))
          {
            RoomHandler roomHandler = this.exitDef.upstreamRoom != this.linkedRoom ? this.exitDef.upstreamRoom : this.exitDef.downstreamRoom;
            if (roomHandler == null || !((UnityEngine.Object) roomHandler.secretRoomManager != (UnityEngine.Object) null) || roomHandler.secretRoomManager.revealStyle != SecretRoomManager.SecretRoomRevealStyle.FireplacePuzzle)
            {
              this.GenerateAmygdala();
              this.m_hasBeenAmygdalaed = true;
              break;
            }
          }
        }
      }

      private void GenerateAmygdala()
      {
        string resourceName = string.Empty;
        Vector2 vector = Vector2.zero;
        switch (this.exitDef.GetDirectionFromRoom(this.linkedRoom))
        {
          case DungeonData.Direction.NORTH:
            resourceName = "Global VFX/Amygdala_South";
            vector = new Vector2(0.0f, 2f);
            break;
          case DungeonData.Direction.EAST:
            resourceName = "Global VFX/Amygdala_West";
            vector = new Vector2(-0.25f, 2f);
            break;
          case DungeonData.Direction.SOUTH:
            resourceName = "Global VFX/Amygdala_North";
            vector = new Vector2(0.0f, 1.5f);
            break;
          case DungeonData.Direction.WEST:
            resourceName = "Global VFX/Amygdala_East";
            vector = new Vector2(0.0f, 2f);
            break;
        }
        this.m_amygdala = (GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire(resourceName));
        this.m_amygdala.transform.position = this.transform.position + vector.ToVector3ZUp();
      }

      private void OnDestroy()
      {
        if (SecretRoomDoorBeer.AllSecretRoomDoors == null)
          return;
        SecretRoomDoorBeer.AllSecretRoomDoors.Remove(this);
      }

      public void SetBreakable()
      {
        if (!(bool) (UnityEngine.Object) this.m_breakable)
          return;
        this.m_breakable.IsSecretDoor = false;
      }

      private void HandlePlayerEnteredLinkedRoom(PlayerController p)
      {
        if (this.exitDef != null)
        {
          RoomHandler roomHandler = this.exitDef.upstreamRoom != this.linkedRoom ? this.exitDef.upstreamRoom : this.exitDef.downstreamRoom;
          if (roomHandler != null && (UnityEngine.Object) roomHandler.secretRoomManager != (UnityEngine.Object) null && roomHandler.secretRoomManager.revealStyle == SecretRoomManager.SecretRoomRevealStyle.FireplacePuzzle)
            return;
        }
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (PassiveItem.ActiveFlagItems.ContainsKey(GameManager.Instance.AllPlayers[index]) && PassiveItem.ActiveFlagItems[GameManager.Instance.AllPlayers[index]].ContainsKey(typeof (SnitchBrickItem)) && !this.m_hasSnitchedBricked)
            this.DoSnitchBrick();
        }
      }

      private void DoSnitchBrick()
      {
        this.m_hasSnitchedBricked = true;
        this.m_snitchBrick = UnityEngine.Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_SnitchBrick"), (Vector3) this.collider.colliderObject.GetComponent<SpeculativeRigidbody>().UnitCenter + DungeonData.GetIntVector2FromDirection(this.exitDef.downstreamExit.referencedExit.exitDirection).ToVector3(), Quaternion.identity);
      }

      public void InitializeFireplace()
      {
      }

      public void InitializeShootToBreak()
      {
        SpeculativeRigidbody component = this.collider.colliderObject.GetComponent<SpeculativeRigidbody>();
        component.PreventPiercing = true;
        this.m_breakable = this.collider.colliderObject.AddComponent<MajorBreakable>();
        this.m_breakable.IsSecretDoor = true;
        this.m_breakable.spawnShards = false;
        this.m_breakable.HitPoints = 25f;
        this.m_breakable.EnemyDamageOverride = 8;
        GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
        if (loadedLevelDefinition != null)
          this.m_breakable.HitPoints *= loadedLevelDefinition.secretDoorHealthMultiplier;
        this.m_breakable.OnDamaged += new Action<float>(this.OnDamaged);
        this.m_breakable.OnBreak += new System.Action(this.OnBreak);
        this.m_breakVfxSprite = UnityEngine.Object.Instantiate<GameObject>(BraveResources.Load<GameObject>("Global VFX/VFX_Secret_Door_Crack_01")).GetComponent<tk2dSprite>();
        this.m_breakFrames = new BreakFrame[2]
        {
          new BreakFrame()
          {
            healthPercentage = 50f,
            sprite = "secret_door_crack_generic{0}_001"
          },
          new BreakFrame()
          {
            healthPercentage = 10f,
            sprite = "secret_door_crack_generic{0}_002"
          }
        };
        if (this.collider.exitDirection == DungeonData.Direction.SOUTH)
        {
          this.m_breakVfxSprite.IsPerpendicular = true;
          this.m_breakVfxSprite.transform.position = (Vector3) component.UnitBottomLeft;
          this.m_breakVfxSprite.HeightOffGround = -1.45f;
          this.m_breakVfxSprite.UpdateZDepth();
        }
        else
        {
          this.m_breakVfxSprite.IsPerpendicular = false;
          this.m_breakVfxSprite.HeightOffGround = 3.2f;
          if (this.collider.exitDirection == DungeonData.Direction.NORTH)
            this.m_breakVfxSprite.transform.position = (Vector3) component.UnitBottomLeft;
          else
            this.m_breakVfxSprite.transform.position = (Vector3) (component.UnitBottomLeft + new Vector2(0.0f, 1f));
          if (this.collider.exitDirection == DungeonData.Direction.EAST)
            this.m_breakVfxSprite.transform.position = this.m_breakVfxSprite.transform.position + new Vector3(-1f, 0.0f, 0.0f);
          this.m_breakVfxSprite.UpdateZDepth();
        }
        this.m_breakVfxSprite.renderer.enabled = false;
        if (!GameManager.Instance.InTutorial)
          return;
        this.m_breakable.MaxHitPoints = this.m_breakable.HitPoints;
        this.m_breakable.HitPoints = 2f;
        this.m_breakable.ApplyDamage(1f, Vector2.zero, false, true, true);
      }

      public void OnDamaged(float damage)
      {
        for (int index = this.m_breakFrames.Length - 1; index >= 0; --index)
        {
          if ((this.m_breakable.MinHits <= 0 || index < this.m_breakable.NumHits) && (double) this.m_breakable.GetCurrentHealthPercentage() <= (double) this.m_breakFrames[index].healthPercentage / 100.0)
          {
            if (!(bool) (UnityEngine.Object) this.m_breakVfxSprite)
              return;
            this.m_breakVfxSprite.renderer.enabled = true;
            this.m_breakVfxSprite.SetSprite(this.GetFrameName(this.m_breakFrames[index].sprite, this.collider.exitDirection));
            return;
          }
        }
        if (!(bool) (UnityEngine.Object) this.m_breakVfxSprite)
          return;
        this.m_breakVfxSprite.renderer.enabled = false;
      }

      public void OnBreak()
      {
        if ((UnityEngine.Object) this.m_snitchBrick != (UnityEngine.Object) null)
        {
          LootEngine.DoDefaultItemPoof(this.m_snitchBrick.GetComponentInChildren<tk2dBaseSprite>().WorldCenter);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_snitchBrick);
        }
        if ((bool) (UnityEngine.Object) this.m_amygdala)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_amygdala);
          this.m_amygdala = (GameObject) null;
        }
        this.BreakOpen();
      }

      public void BreakOpen()
      {
        if ((bool) (UnityEngine.Object) this.m_breakVfxSprite)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_breakVfxSprite);
        int num = (int) AkSoundEngine.PostEvent("Play_UI_secret_reveal_01", this.gameObject);
        this.manager.IsOpen = true;
        this.manager.HandleDoorBrokenOpen(this);
        this.collider.colliderObject.GetComponent<SpeculativeRigidbody>().enabled = false;
        if (this.wallChunks == null)
          return;
        for (int index = 0; index < this.wallChunks.Count; ++index)
        {
          this.wallChunks[index].gameObject.SetActive(true);
          this.wallChunks[index].Trigger();
        }
      }

      public void GeneratePotentiallyNecessaryShards()
      {
        GameObject wallShardCollection = GameManager.Instance.Dungeon.roomMaterialDefinitions[this.manager.room.RoomVisualSubtype].GetSecretRoomWallShardCollection();
        if (!((UnityEngine.Object) wallShardCollection != (UnityEngine.Object) null))
          return;
        GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(wallShardCollection);
        gameObject1.transform.position = this.transform.position;
        while (gameObject1.transform.childCount > 0)
        {
          GameObject gameObject2 = gameObject1.transform.GetChild(0).gameObject;
          gameObject2.transform.parent = this.transform;
          if (this.wallChunks == null)
            this.wallChunks = new List<BreakableChunk>();
          gameObject2.SetActive(false);
          this.wallChunks.Add(gameObject2.GetComponent<BreakableChunk>());
        }
      }

      private string GetFrameName(string name, DungeonData.Direction dir)
      {
        if (!name.Contains("{0}"))
          return name;
        string str;
        switch (dir)
        {
          case DungeonData.Direction.NORTH:
            str = "_top_top";
            break;
          case DungeonData.Direction.EAST:
            str = "_right_top";
            break;
          default:
            str = dir == DungeonData.Direction.WEST ? "_left_top" : string.Empty;
            break;
        }
        return string.Format(name, (object) str);
      }
    }

}
