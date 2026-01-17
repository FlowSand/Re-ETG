// Decompiled with JetBrains decompiler
// Type: FlippableCover
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
    public class FlippableCover : BraveBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
      public FlippableCover.FlipStyle flipStyle;
      public tk2dSprite shadowSprite;
      public float DamageReceivedOnSlide = 30f;
      [Header("Unflipped Animations")]
      public string unflippedBreakAnimation;
      [Header("Directional Animations")]
      [HelpBox("{0} = east/west/south/north")]
      public string flipAnimation;
      public string shadowFlipAnimation;
      public string pitfallAnimation;
      public string breakAnimation;
      public BreakFrame[] prebreakFrames;
      public BreakFrame[] prebreakFramesUnflipped;
      public bool BreaksOnBreakAnimation;
      [Header("SubElements (for coffins)")]
      public List<FlippableSubElement> flipSubElements = new List<FlippableSubElement>();
      [Header("Directional Outline Sprite")]
      public GameObject outlineNorth;
      public GameObject outlineEast;
      public GameObject outlineSouth;
      public GameObject outlineWest;
      public bool UsesCustomHeightsOffGround;
      public float CustomStartHeightOffGround;
      public float CustomNorthFlippedHeightOffGround = -1.5f;
      public float CustomEastFlippedHeightOffGround = -1.5f;
      public float CustomSouthFlippedHeightOffGround = -1.5f;
      public float CustomWestFlippedHeightOffGround = -1.5f;
      public bool DelayMoveable;
      public float MoveableDelay = 1f;
      public float VibrationDelay = 0.25f;
      private SlideSurface m_slide;
      private bool m_hasRoomEnteredProcessed;
      private bool m_isGilded;
      private PlayerController m_flipperPlayer;
      protected tk2dSpriteAnimator m_shadowSpriteAnimator;
      protected OccupiedCells m_occupiedCells;
      protected MajorBreakable m_breakable;
      protected bool m_flipped;
      protected DungeonData.Direction m_flipDirection;
      protected bool m_shouldDisplayOutline;
      protected PlayerController m_lastInteractingPlayer;
      protected DungeonData.Direction m_lastOutlineDirection = ~DungeonData.Direction.NORTH;
      protected float m_makeBreakableTimer = -1f;

      public bool IsBroken
      {
        get => !((UnityEngine.Object) this.m_breakable == (UnityEngine.Object) null) && this.m_breakable.IsDestroyed;
      }

      public bool IsFlipped => this.m_flipped;

      public DungeonData.Direction DirectionFlipped => this.m_flipDirection;

      public bool PreventPitFalls { get; set; }

      public void Awake()
      {
        this.specRigidbody = this.GetComponentInChildren<SpeculativeRigidbody>();
        this.m_slide = this.GetComponentInChildren<SlideSurface>();
      }

      private void Start()
      {
        if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
          this.sprite = (tk2dBaseSprite) this.transform.GetChild(0).GetComponent<tk2dSprite>();
        if ((UnityEngine.Object) this.spriteAnimator == (UnityEngine.Object) null)
          this.spriteAnimator = this.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
        this.sprite.AdditionalFlatForwardPercentage = 0.125f;
        this.sprite.IsPerpendicular = this.flipStyle == FlippableCover.FlipStyle.NO_FLIPS && this.sprite.IsPerpendicular;
        this.sprite.HeightOffGround = !this.UsesCustomHeightsOffGround ? 0.0f : this.CustomStartHeightOffGround;
        this.sprite.UpdateZDepth();
        if ((UnityEngine.Object) this.shadowSprite != (UnityEngine.Object) null)
        {
          this.shadowSprite.IsPerpendicular = false;
          this.shadowSprite.usesOverrideMaterial = true;
          this.shadowSprite.HeightOffGround = -1f;
          this.shadowSprite.UpdateZDepth();
          this.m_shadowSpriteAnimator = this.shadowSprite.GetComponent<tk2dSpriteAnimator>();
        }
        this.m_breakable = this.GetComponentInChildren<MajorBreakable>();
        if ((UnityEngine.Object) this.m_breakable != (UnityEngine.Object) null)
        {
          this.m_breakable.OnDamaged += new Action<float>(this.Damaged);
          this.m_breakable.OnBreak += new System.Action(this.DestroyCover);
          if (this.prebreakFrames.Length > 0)
            this.m_breakable.MinHitPointsFromNonExplosions = 1f;
        }
        this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
        this.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostMovement);
        if (this.specRigidbody.PixelColliders.Count > 1)
          this.specRigidbody.PixelColliders[1].Enabled = false;
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        if (absoluteRoom != null)
        {
          absoluteRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.HandleParentRoomEntered);
          if ((bool) (UnityEngine.Object) GameManager.Instance.BestActivePlayer && absoluteRoom == GameManager.Instance.BestActivePlayer.CurrentRoom)
            this.m_hasRoomEnteredProcessed = true;
        }
        if (this.flipStyle != FlippableCover.FlipStyle.NO_FLIPS)
          return;
        this.RemoveFromRoomHierarchy();
        this.specRigidbody.CanBePushed = true;
      }

      private void HandleParentRoomEntered(PlayerController p)
      {
        if (this.m_hasRoomEnteredProcessed)
          return;
        this.m_hasRoomEnteredProcessed = true;
        if (!(bool) (UnityEngine.Object) p || !p.HasActiveBonusSynergy(CustomSynergyType.GILDED_TABLES) || (double) UnityEngine.Random.value >= 0.15000000596046448)
          return;
        this.m_isGilded = true;
        this.sprite.usesOverrideMaterial = true;
        tk2dSprite sprite = this.sprite as tk2dSprite;
        sprite.GenerateUV2 = true;
        Material material1 = UnityEngine.Object.Instantiate<Material>(this.sprite.renderer.material);
        material1.DisableKeyword("TINTING_OFF");
        material1.EnableKeyword("TINTING_ON");
        material1.SetColor("_OverrideColor", new Color(1f, 0.77f, 0.0f));
        material1.DisableKeyword("EMISSIVE_OFF");
        material1.EnableKeyword("EMISSIVE_ON");
        material1.SetFloat("_EmissivePower", 1.75f);
        material1.SetFloat("_EmissiveColorPower", 1f);
        this.sprite.renderer.material = material1;
        Shader shader = Shader.Find("Brave/ItemSpecific/MetalSkinLayerShader");
        MeshRenderer component = this.sprite.GetComponent<MeshRenderer>();
        Material[] sharedMaterials = component.sharedMaterials;
        for (int index = 0; index < sharedMaterials.Length; ++index)
        {
          if ((UnityEngine.Object) sharedMaterials[index].shader == (UnityEngine.Object) shader)
            return;
        }
        Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
        Material material2 = new Material(shader);
        material2.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
        sharedMaterials[sharedMaterials.Length - 1] = material2;
        component.sharedMaterials = sharedMaterials;
        sprite.ForceBuild();
      }

      protected void ClearOutlines()
      {
        this.outlineNorth.SetActive(false);
        this.outlineEast.SetActive(false);
        this.outlineSouth.SetActive(false);
        this.outlineWest.SetActive(false);
        this.m_lastOutlineDirection = ~DungeonData.Direction.NORTH;
      }

      protected void ToggleOutline(DungeonData.Direction dir)
      {
        if (this.IsBroken || this.flipStyle == FlippableCover.FlipStyle.NO_FLIPS)
          return;
        switch (dir)
        {
          case DungeonData.Direction.NORTH:
            if (this.flipStyle != FlippableCover.FlipStyle.ONLY_FLIPS_LEFT_RIGHT)
            {
              this.outlineNorth.SetActive(!this.outlineNorth.activeSelf);
              break;
            }
            break;
          case DungeonData.Direction.EAST:
            if (this.flipStyle != FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN)
            {
              this.outlineEast.SetActive(!this.outlineEast.activeSelf);
              break;
            }
            break;
          case DungeonData.Direction.SOUTH:
            if (this.flipStyle != FlippableCover.FlipStyle.ONLY_FLIPS_LEFT_RIGHT)
            {
              this.outlineSouth.SetActive(!this.outlineSouth.activeSelf);
              break;
            }
            break;
          case DungeonData.Direction.WEST:
            if (this.flipStyle != FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN)
            {
              this.outlineWest.SetActive(!this.outlineWest.activeSelf);
              break;
            }
            break;
        }
        this.sprite.UpdateZDepth();
      }

      private void Update()
      {
        if (this.spriteAnimator.IsPlaying(this.spriteAnimator.CurrentClip))
        {
          this.spriteAnimator.ForceInvisibleSpriteUpdate();
          if ((bool) (UnityEngine.Object) this.specRigidbody)
            this.specRigidbody.ForceRegenerate();
        }
        if (this.m_shouldDisplayOutline)
        {
          DungeonData.Direction inverseDirection = DungeonData.GetInverseDirection(this.GetFlipDirection(this.m_lastInteractingPlayer.specRigidbody));
          if (inverseDirection != this.m_lastOutlineDirection)
          {
            this.ToggleOutline(this.m_lastOutlineDirection);
            this.ToggleOutline(inverseDirection);
          }
          this.m_lastOutlineDirection = inverseDirection;
        }
        if ((double) this.m_makeBreakableTimer <= 0.0)
          return;
        this.m_makeBreakableTimer -= BraveTime.DeltaTime;
        if ((double) this.m_makeBreakableTimer > 0.0)
          return;
        this.m_breakable.MinHitPointsFromNonExplosions = 0.0f;
        if (this.m_flipped || !(bool) (UnityEngine.Object) this.m_breakable || GameManager.Instance.InTutorial)
          return;
        this.m_breakable.ApplyDamage(this.DamageReceivedOnSlide, Vector2.zero, false);
      }

      protected override void OnDestroy() => base.OnDestroy();

      public float GetDistanceToPoint(Vector2 point)
      {
        if ((UnityEngine.Object) this.specRigidbody == (UnityEngine.Object) null || (UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
          return 100f;
        Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, (Vector2) this.sprite.GetBounds().size);
        return Vector2.Distance(point, (Vector2) b) / 1.5f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        this.m_lastInteractingPlayer = interactor;
        if (!(bool) (UnityEngine.Object) this)
          return;
        this.m_shouldDisplayOutline = true;
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        this.ClearOutlines();
        this.m_shouldDisplayOutline = false;
      }

      public void Interact(PlayerController player)
      {
        this.Flip(player.specRigidbody);
        player.DoVibration(Vibration.Time.Quick, Vibration.Strength.UltraLight);
        this.ClearOutlines();
        this.m_shouldDisplayOutline = false;
      }

      public DungeonData.Direction GetFlipDirection(SpeculativeRigidbody flipperRigidbody)
      {
        bool flag1 = (double) flipperRigidbody.UnitRight <= (double) this.specRigidbody.UnitLeft;
        bool flag2 = (double) flipperRigidbody.UnitLeft >= (double) this.specRigidbody.UnitRight;
        bool flag3 = (double) flipperRigidbody.UnitBottom >= (double) this.specRigidbody.UnitTop;
        bool flag4 = (double) flipperRigidbody.UnitTop <= (double) this.specRigidbody.UnitBottom;
        if (flag1 && !flag3 && !flag4)
          return DungeonData.Direction.EAST;
        if (flag2 && !flag3 && !flag4)
          return DungeonData.Direction.WEST;
        if (flag3 && !flag1 && !flag2)
          return DungeonData.Direction.SOUTH;
        if (flag4 && !flag1 && !flag2)
          return DungeonData.Direction.NORTH;
        Vector2 vector2_1 = Vector2.zero;
        Vector2 vector2_2 = Vector2.zero;
        PlayerController component = flipperRigidbody.GetComponent<PlayerController>();
        bool flag5 = (bool) (UnityEngine.Object) component && component.IsSlidingOverSurface;
        if (flag1 && flag3)
        {
          vector2_1 = flipperRigidbody.UnitBottomRight;
          vector2_2 = this.specRigidbody.UnitTopLeft;
        }
        else if (flag2 && flag3)
        {
          vector2_1 = flipperRigidbody.UnitBottomLeft;
          vector2_2 = this.specRigidbody.UnitTopRight;
        }
        else if (flag1 && flag4)
        {
          vector2_1 = flipperRigidbody.UnitTopRight;
          vector2_2 = this.specRigidbody.UnitBottomLeft;
        }
        else if (flag2 && flag4)
        {
          vector2_1 = flipperRigidbody.UnitTopLeft;
          vector2_2 = this.specRigidbody.UnitBottomRight;
        }
        else if ((bool) (UnityEngine.Object) this.m_slide && flag5)
        {
          vector2_1 = flipperRigidbody.UnitCenter;
          vector2_2 = this.specRigidbody.UnitCenter;
        }
        else
          UnityEngine.Debug.LogError((object) "Something about this table and flipper is TOTALLY WRONG MAN (way #1)");
        Vector2 vector = vector2_1 - vector2_2;
        if (vector == Vector2.zero)
        {
          if (flag4)
            return DungeonData.Direction.NORTH;
          if (flag3)
            return DungeonData.Direction.SOUTH;
        }
        if ((bool) (UnityEngine.Object) this.m_slide && flag5)
        {
          vector = -component.Velocity;
          if (!component.IsSlidingOverSurface)
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_SLID_OVER_TABLE, 1f);
          component.IsSlidingOverSurface = true;
          if (!component.TablesDamagedThisSlide.Contains(this))
          {
            component.TablesDamagedThisSlide.Add(this);
            if ((bool) (UnityEngine.Object) this.m_breakable && !GameManager.Instance.InTutorial)
              this.m_breakable.ApplyDamage(this.DamageReceivedOnSlide, Vector2.zero, false);
          }
        }
        Vector2 majorAxis = BraveUtility.GetMajorAxis(vector);
        if ((double) majorAxis.x < 0.0)
          return DungeonData.Direction.EAST;
        if ((double) majorAxis.x > 0.0)
          return DungeonData.Direction.WEST;
        if ((double) majorAxis.y < 0.0)
          return DungeonData.Direction.NORTH;
        if ((double) majorAxis.y > 0.0)
          return DungeonData.Direction.SOUTH;
        UnityEngine.Debug.LogError((object) "Something about this table and flipper is TOTALLY WRONG MAN (way #2)");
        return DungeonData.Direction.NORTH;
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        switch (this.GetFlipDirection(interactor.specRigidbody))
        {
          case DungeonData.Direction.NORTH:
            return "tablekick_up";
          case DungeonData.Direction.EAST:
            return "tablekick_right";
          case DungeonData.Direction.SOUTH:
            return "tablekick_down";
          case DungeonData.Direction.WEST:
            shouldBeFlipped = true;
            return "tablekick_right";
          default:
            return "error";
        }
      }

      private void MakePerpendicularOnFlipped(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
      {
        this.sprite.IsPerpendicular = true;
        if (this.m_flipDirection == DungeonData.Direction.NORTH || this.m_flipDirection == DungeonData.Direction.SOUTH)
        {
          float num = this.m_flipDirection != DungeonData.Direction.NORTH ? this.CustomSouthFlippedHeightOffGround : this.CustomNorthFlippedHeightOffGround;
          this.sprite.HeightOffGround = !this.UsesCustomHeightsOffGround ? -1.5f : num;
          if ((UnityEngine.Object) this.shadowSprite != (UnityEngine.Object) null)
            this.shadowSprite.HeightOffGround = !this.UsesCustomHeightsOffGround ? -1.5f : -1.75f;
        }
        else
        {
          float num = this.m_flipDirection != DungeonData.Direction.EAST ? this.CustomWestFlippedHeightOffGround : this.CustomEastFlippedHeightOffGround;
          this.sprite.HeightOffGround = !this.UsesCustomHeightsOffGround ? -1f : num;
          if ((UnityEngine.Object) this.shadowSprite != (UnityEngine.Object) null)
            this.shadowSprite.HeightOffGround = -1.5f;
        }
        this.sprite.UpdateZDepth();
        if ((UnityEngine.Object) this.shadowSprite != (UnityEngine.Object) null)
          this.shadowSprite.UpdateZDepth();
        this.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.MakePerpendicularOnFlipped);
      }

      [DebuggerHidden]
      private IEnumerator DelayedMakePerpendicular(float time)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FlippableCover.<DelayedMakePerpendicular>c__Iterator0()
        {
          time = time,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator DelayedBreakBreakables(float time)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FlippableCover.<DelayedBreakBreakables>c__Iterator1()
        {
          time = time,
          _this = this
        };
      }

      public void Flip(DungeonData.Direction flipDirection)
      {
        if (this.IsFlipped)
          return;
        if (GameManager.Instance.InTutorial)
          GameManager.BroadcastRoomTalkDoerFsmEvent("playerFlippedTable");
        int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_table_flip_01", this.gameObject);
        GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY()).DeregisterInteractable((IPlayerInteractable) this);
        if ((UnityEngine.Object) this.m_breakable != (UnityEngine.Object) null)
          this.m_breakable.TriggerTemporaryDestructibleVFXClear();
        this.m_flipDirection = flipDirection;
        if (!string.IsNullOrEmpty(this.flipAnimation))
        {
          this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.FlipCompleted);
          this.spriteAnimator.Play(this.GetAnimName(this.flipAnimation, this.m_flipDirection));
          if (this.m_flipDirection == DungeonData.Direction.NORTH)
            this.MakePerpendicularOnFlipped((tk2dSpriteAnimator) null, (tk2dSpriteAnimationClip) null);
          else
            this.StartCoroutine(this.DelayedMakePerpendicular(this.spriteAnimator.CurrentClip.BaseClipLength / 2.25f));
          this.StartCoroutine(this.DelayedBreakBreakables(this.spriteAnimator.CurrentClip.BaseClipLength / 2f));
          if (this.m_flipDirection == DungeonData.Direction.SOUTH)
            this.sprite.IsPerpendicular = true;
        }
        else
        {
          this.sprite.IsPerpendicular = true;
          if (this.m_flipDirection == DungeonData.Direction.NORTH || this.m_flipDirection == DungeonData.Direction.SOUTH)
          {
            float num2 = this.m_flipDirection != DungeonData.Direction.NORTH ? this.CustomSouthFlippedHeightOffGround : this.CustomNorthFlippedHeightOffGround;
            this.sprite.HeightOffGround = !this.UsesCustomHeightsOffGround ? -1.5f : num2;
          }
          this.BreakBreakablesFlippedUpon(this.m_flipDirection);
          this.FlipCompleted((tk2dSpriteAnimator) null, (tk2dSpriteAnimationClip) null);
        }
        if ((bool) (UnityEngine.Object) this.m_flipperPlayer && this.m_flipperPlayer.OnTableFlipped != null)
          this.m_flipperPlayer.OnTableFlipped(this);
        if (!string.IsNullOrEmpty(this.shadowFlipAnimation) && (UnityEngine.Object) this.m_shadowSpriteAnimator != (UnityEngine.Object) null)
          this.m_shadowSpriteAnimator.Play(this.GetAnimName(this.shadowFlipAnimation, this.m_flipDirection));
        bool flag = false;
        for (int index = 0; index < this.flipSubElements.Count; ++index)
        {
          if ((this.flipSubElements[index].isMandatory || (double) UnityEngine.Random.value < (double) this.flipSubElements[index].spawnChance) && (!this.flipSubElements[index].requiresDirection || this.flipSubElements[index].requiredDirection == flipDirection))
          {
            if (this.flipSubElements[index].onlyOneOfThese)
            {
              if (!flag)
                flag = true;
              else
                continue;
            }
            this.StartCoroutine(this.ProcessSubElement(this.flipSubElements[index], flipDirection));
          }
        }
        this.m_occupiedCells.UpdateCells();
        if (this.DelayMoveable)
          this.StartCoroutine(this.HandleDelayedMoveability());
        else
          this.specRigidbody.CanBePushed = true;
        if ((bool) (UnityEngine.Object) this.m_flipperPlayer)
          this.StartCoroutine(this.HandleDelayedVibration(this.m_flipperPlayer));
        if (this.specRigidbody.PixelColliders.Count >= 2)
          this.specRigidbody.PixelColliders[1].Enabled = true;
        this.m_flipped = true;
        this.sprite.UpdateZDepth();
        if ((bool) (UnityEngine.Object) this.shadowSprite)
          this.shadowSprite.UpdateZDepth();
        SurfaceDecorator component = this.GetComponent<SurfaceDecorator>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
          return;
        component.Destabilize(DungeonData.GetIntVector2FromDirection(this.m_flipDirection).ToVector2());
      }

      [DebuggerHidden]
      private IEnumerator HandleDelayedMoveability()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FlippableCover.<HandleDelayedMoveability>c__Iterator2()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleDelayedVibration(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FlippableCover.<HandleDelayedVibration>c__Iterator3()
        {
          player = player,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ProcessSubElement(
        FlippableSubElement element,
        DungeonData.Direction flipDirection)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FlippableCover.<ProcessSubElement>c__Iterator4()
        {
          element = element,
          flipDirection = flipDirection,
          _this = this
        };
      }

      public void ForceSetFlipper(PlayerController flipper) => this.m_flipperPlayer = flipper;

      public void Flip(SpeculativeRigidbody flipperRigidbody)
      {
        if (this.IsFlipped)
          return;
        this.specRigidbody.PixelColliders[1].Enabled = true;
        this.RemoveFromRoomHierarchy();
        DungeonData.Direction flipDirection = this.GetFlipDirection(flipperRigidbody);
        if (this.flipStyle == FlippableCover.FlipStyle.NO_FLIPS)
          return;
        if (this.flipStyle == FlippableCover.FlipStyle.ONLY_FLIPS_LEFT_RIGHT)
        {
          if (flipDirection == DungeonData.Direction.NORTH || flipDirection == DungeonData.Direction.SOUTH)
            return;
        }
        else if (this.flipStyle == FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN && (flipDirection == DungeonData.Direction.EAST || flipDirection == DungeonData.Direction.WEST))
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_table_flip_01", this.gameObject);
        if ((UnityEngine.Object) this.m_breakable != (UnityEngine.Object) null)
          this.m_breakable.TriggerTemporaryDestructibleVFXClear();
        if ((bool) (UnityEngine.Object) flipperRigidbody.gameActor && flipperRigidbody.gameActor is PlayerController)
        {
          this.m_flipperPlayer = flipperRigidbody.gameActor as PlayerController;
          this.ForceBlank(2f);
          this.m_flipperPlayer.healthHaver.TriggerInvulnerabilityPeriod();
        }
        this.Flip(flipDirection);
        if (!(flipperRigidbody.gameActor is PlayerController))
          return;
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TABLES_FLIPPED, 1f);
      }

      public bool IsGilded => this.m_isGilded;

      private void FlipCompleted(
        tk2dSpriteAnimator tk2DSpriteAnimator,
        tk2dSpriteAnimationClip tk2DSpriteAnimationClip)
      {
        this.m_occupiedCells.UpdateCells();
        this.sprite.UpdateZDepth();
        if ((bool) (UnityEngine.Object) this.m_flipperPlayer && this.m_flipperPlayer.OnTableFlipCompleted != null)
          this.m_flipperPlayer.OnTableFlipCompleted(this);
        if (this.m_isGilded)
        {
          RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
          if (absoluteRoom != null)
          {
            List<AIActor> activeEnemies = absoluteRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) activeEnemies[index] && activeEnemies[index].IsNormalEnemy)
                activeEnemies[index].AssignedCurrencyToDrop += UnityEngine.Random.Range(2, 6);
            }
          }
          this.m_isGilded = false;
        }
        tk2DSpriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.FlipCompleted);
      }

      private void BreakBreakablesFlippedUpon(DungeonData.Direction flipDirection)
      {
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        for (int index = 0; index < StaticReferenceManager.AllMinorBreakables.Count; ++index)
        {
          if (!StaticReferenceManager.AllMinorBreakables[index].IsBroken && !(bool) (UnityEngine.Object) StaticReferenceManager.AllMinorBreakables[index].debris && StaticReferenceManager.AllMinorBreakables[index].transform.position.GetAbsoluteRoom() == absoluteRoom)
          {
            SpeculativeRigidbody specRigidbody = StaticReferenceManager.AllMinorBreakables[index].specRigidbody;
            if ((bool) (UnityEngine.Object) specRigidbody && (bool) (UnityEngine.Object) this.specRigidbody && (double) BraveMathCollege.DistBetweenRectangles(specRigidbody.UnitBottomLeft, specRigidbody.UnitDimensions, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions) < 0.5)
              StaticReferenceManager.AllMinorBreakables[index].Break();
          }
        }
      }

      private void RemoveFromRoomHierarchy()
      {
        Transform hierarchyParent = this.transform.position.GetAbsoluteRoom().hierarchyParent;
        for (Transform transform = this.transform; (UnityEngine.Object) transform.parent != (UnityEngine.Object) null; transform = transform.parent)
        {
          if ((UnityEngine.Object) transform.parent == (UnityEngine.Object) hierarchyParent)
          {
            transform.parent = (Transform) null;
            break;
          }
        }
      }

      public void Damaged(float damage)
      {
        if (this.m_flipped || this.prebreakFramesUnflipped == null || this.prebreakFramesUnflipped.Length == 0)
        {
          for (int index = this.prebreakFrames.Length - 1; index >= 0; --index)
          {
            if ((double) this.m_breakable.GetCurrentHealthPercentage() <= (double) this.prebreakFrames[index].healthPercentage / 100.0)
            {
              if (this.m_flipped)
                this.sprite.SetSprite(this.GetAnimName(this.prebreakFrames[index].sprite, this.m_flipDirection));
              if (index != this.prebreakFrames.Length - 1 || (double) this.m_makeBreakableTimer > 0.0)
                break;
              this.m_makeBreakableTimer = 0.5f;
              break;
            }
          }
        }
        else
        {
          for (int index = this.prebreakFramesUnflipped.Length - 1; index >= 0; --index)
          {
            if ((double) this.m_breakable.GetCurrentHealthPercentage() <= (double) this.prebreakFramesUnflipped[index].healthPercentage / 100.0)
            {
              this.sprite.SetSprite(this.prebreakFramesUnflipped[index].sprite);
              if (index != this.prebreakFramesUnflipped.Length - 1 || (double) this.m_makeBreakableTimer > 0.0)
                break;
              this.m_makeBreakableTimer = 0.5f;
              break;
            }
          }
        }
      }

      public void DestroyCover() => this.DestroyCover(false, new IntVector2?());

      public void DestroyCover(bool fellInPit, IntVector2? pushDirection)
      {
        if (!this.m_flipped)
        {
          SurfaceDecorator component = this.GetComponent<SurfaceDecorator>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            component.Destabilize(Vector2.zero);
          this.ClearOutlines();
          GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY()).DeregisterInteractable((IPlayerInteractable) this);
        }
        this.m_occupiedCells.Clear();
        if (fellInPit && pushDirection.HasValue)
        {
          this.StartCoroutine(this.StartFallAnimation(pushDirection.Value));
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.PUSH_TABLE_INTO_PIT);
        }
        else if (!this.m_flipped && this.spriteAnimator.GetClipByName(this.unflippedBreakAnimation) == null)
        {
          LootEngine.DoDefaultPurplePoof(this.sprite.WorldCenter);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
        {
          this.spriteAnimator.Play(this.m_flipped ? this.GetAnimName(this.breakAnimation, this.m_flipDirection) : this.unflippedBreakAnimation);
          if (this.BreaksOnBreakAnimation)
            this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DestroyBrokenTable);
        }
        if ((bool) (UnityEngine.Object) this.shadowSprite)
          this.shadowSprite.renderer.enabled = false;
        this.sprite.IsPerpendicular = false;
        this.sprite.HeightOffGround = -1.25f;
        this.sprite.UpdateZDepth();
        this.ForceBlank(2f);
      }

      private void DestroyBrokenTable(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      public void ForceBlank(float overrideRadius = 25f, float overrideTimeAtMaxRadius = 0.5f)
      {
        SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
        silencerInstance.ForceNoDamage = true;
        silencerInstance.TriggerSilencer(this.specRigidbody.UnitCenter, 50f, overrideRadius, (GameObject) null, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, overrideTimeAtMaxRadius, (PlayerController) null, false, true);
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.specRigidbody.Initialize();
        this.m_occupiedCells = new OccupiedCells(this.specRigidbody, room);
      }

      protected virtual void OnRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        PlayerController component = rigidbodyCollision.OtherRigidbody.GetComponent<PlayerController>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !rigidbodyCollision.Overlap)
          return;
        component.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
      }

      protected virtual void OnPostMovement(
        SpeculativeRigidbody rigidbody,
        Vector2 unitOffset,
        IntVector2 pixelOffset)
      {
        if (pixelOffset != IntVector2.Zero)
          this.CheckForPitDeath(pixelOffset);
        if (rigidbody.CanBePushed && (bool) (UnityEngine.Object) this.sprite)
          this.sprite.UpdateZDepth();
        if ((bool) (UnityEngine.Object) this.shadowSprite)
        {
          this.shadowSprite.transform.localPosition = this.sprite.transform.localPosition;
          this.shadowSprite.UpdateZDepth();
        }
        if (!(unitOffset != Vector2.zero))
          return;
        this.m_occupiedCells.UpdateCells();
      }

      private string GetAnimName(string name, DungeonData.Direction dir)
      {
        return name.Contains("{0}") ? string.Format(name, (object) dir.ToString().ToLower()) : name;
      }

      private void CheckForPitDeath(IntVector2 dir)
      {
        if (this.specRigidbody.PixelColliders.Count == 0 || this.PreventPitFalls)
          return;
        Rect rect = new Rect();
        rect.min = this.specRigidbody.PixelColliders[0].UnitBottomLeft;
        rect.max = this.specRigidbody.PixelColliders[0].UnitTopRight;
        for (int index = 1; index < this.specRigidbody.PixelColliders.Count; ++index)
        {
          rect.min = Vector2.Min(rect.min, this.specRigidbody.PixelColliders[index].UnitBottomLeft);
          rect.max = Vector2.Max(rect.max, this.specRigidbody.PixelColliders[index].UnitTopRight);
        }
        Dungeon dungeon = GameManager.Instance.Dungeon;
        List<IntVector2> intVector2List = new List<IntVector2>();
        if (dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMin, rect.yMin)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMin, rect.yMax)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.center.x, rect.yMin)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.center.x, rect.yMax)))
          intVector2List.Add(IntVector2.Left);
        else if (dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMax, rect.yMin)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMax, rect.yMax)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.center.x, rect.yMin)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.center.x, rect.yMax)))
          intVector2List.Add(IntVector2.Right);
        else if (dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMin, rect.yMax)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMax, rect.yMax)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMin, rect.center.y)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMax, rect.center.y)))
          intVector2List.Add(IntVector2.Up);
        else if (dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMin, rect.yMin)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMax, rect.yMin)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMin, rect.center.y)) && dungeon.CellSupportsFalling((Vector3) new Vector2(rect.xMax, rect.center.y)))
          intVector2List.Add(IntVector2.Down);
        if (intVector2List.Count <= 0)
          return;
        this.DestroyCover(true, new IntVector2?(!intVector2List.Contains(dir.MajorAxis) ? intVector2List[0] : dir.MajorAxis));
      }

      [DebuggerHidden]
      private IEnumerator StartFallAnimation(IntVector2 dir)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new FlippableCover.<StartFallAnimation>c__Iterator5()
        {
          dir = dir,
          _this = this
        };
      }

      public enum FlipStyle
      {
        ANY,
        ONLY_FLIPS_UP_DOWN,
        ONLY_FLIPS_LEFT_RIGHT,
        NO_FLIPS,
      }
    }

}
