// Decompiled with JetBrains decompiler
// Type: HangingObjectController
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

namespace ETG.Core.Dungeon.Interactables
{
    public class HangingObjectController : 
      DungeonPlaceableBehaviour,
      IPlayerInteractable,
      IEventTriggerable,
      IPlaceConfigurable
    {
      public tk2dSprite objectSprite;
      public tk2dSpriteAnimator AdditionalChainDownAnimator;
      public bool destroyOnFinish = true;
      public GameObject[] additionalDestroyObjects;
      public bool hasSubSprites;
      public tk2dSprite[] subSprites;
      public float startingHeight = 5f;
      public bool DoExplosion = true;
      public ExplosionData explosionData;
      public Vector3 explosionOffset;
      public GameObject replacementRangeEffect;
      public GameObject triggerObjectPrefab;
      public bool DoesTriggerShake;
      public ScreenShakeSettings TriggerShake;
      public SpeculativeRigidbody EnableRigidbodyPostFall;
      public bool MakeMajorBreakableAfterwards;
      protected Transform m_objectTransform;
      protected Vector3 m_cachedStartingPosition;
      protected float m_currentHeight;
      protected bool m_hasFallen;
      protected RoomHandler m_room;
      protected MinorBreakable TriggerObjectBreakable;
      protected tk2dSpriteAnimator TriggerObjectAnimator;
      protected tk2dSpriteAnimator TriggerChainAnimator;
      private bool m_subsidiary;
      private float GRAVITY_ACCELERATION = -10f;
      private int subspritesFalling;
      private HashSet<float> m_usedDepths;

      private void Start()
      {
        this.m_currentHeight = this.startingHeight;
        this.m_objectTransform = this.objectSprite.transform;
        this.m_cachedStartingPosition = this.m_objectTransform.position;
        this.objectSprite.HeightOffGround = this.m_currentHeight + 1f;
        this.m_objectTransform.position = this.m_cachedStartingPosition + new Vector3(0.0f, this.m_currentHeight, 0.0f);
        this.objectSprite.UpdateZDepth();
        if (!((UnityEngine.Object) this.triggerObjectPrefab != (UnityEngine.Object) null))
          return;
        RoomEventTriggerArea triggerAreaFromObject = this.m_room.GetEventTriggerAreaFromObject((IEventTriggerable) this);
        if (triggerAreaFromObject == null)
          return;
        if ((UnityEngine.Object) triggerAreaFromObject.tempDataObject != (UnityEngine.Object) null)
        {
          this.m_subsidiary = true;
          this.TriggerObjectBreakable = triggerAreaFromObject.tempDataObject.GetComponentInChildren<MinorBreakable>();
          this.TriggerObjectBreakable.OnlyBreaksOnScreen = true;
          this.TriggerObjectBreakable.OnBreak += new System.Action(this.TriggerFallBroken);
          this.TriggerObjectBreakable.GetComponent<tk2dSpriteAnimator>().OnPlayAnimationCalled += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.SubTriggerAnim);
        }
        else
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.triggerObjectPrefab, triggerAreaFromObject.initialPosition.ToVector3((float) triggerAreaFromObject.initialPosition.y), Quaternion.identity);
          this.TriggerObjectBreakable = gameObject.GetComponentInChildren<MinorBreakable>();
          this.TriggerObjectBreakable.OnlyBreaksOnScreen = true;
          this.TriggerObjectBreakable.OnBreak += new System.Action(this.TriggerFallBroken);
          this.TriggerObjectAnimator = this.TriggerObjectBreakable.GetComponent<tk2dSpriteAnimator>();
          if (this.TriggerObjectBreakable.transform.childCount > 0)
            this.TriggerChainAnimator = this.TriggerObjectBreakable.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
          triggerAreaFromObject.tempDataObject = gameObject;
          if (!(bool) (UnityEngine.Object) this.TriggerObjectBreakable || !(bool) (UnityEngine.Object) this.TriggerObjectBreakable.sprite)
            return;
          this.TriggerObjectBreakable.sprite.IsPerpendicular = true;
          this.TriggerObjectBreakable.sprite.UpdateZDepth();
        }
      }

      private void SubTriggerAnim(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
      {
        this.m_subsidiary = true;
        this.Interact(GameManager.Instance.BestActivePlayer);
      }

      public void Trigger(int index)
      {
        if (!((UnityEngine.Object) this.triggerObjectPrefab == (UnityEngine.Object) null))
          return;
        this.TriggerFallBroken();
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_room = room;
        if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON || room.RoomVisualSubtype != 6)
          return;
        room.RoomVisualSubtype = (double) UnityEngine.Random.value <= 0.5 ? 3 : 0;
        for (int index = 0; index < room.Cells.Count; ++index)
          room.UpdateCellVisualData(room.Cells[index].x, room.Cells[index].y);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if (this.m_hasFallen || (UnityEngine.Object) this.TriggerObjectAnimator == (UnityEngine.Object) null)
          return 1000f;
        tk2dBaseSprite sprite = this.TriggerObjectAnimator.Sprite;
        return Vector2.Distance(point, sprite.WorldCenter) / 2f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        SpriteOutlineManager.AddOutlineToSprite(this.TriggerObjectAnimator.Sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.TriggerObjectAnimator.Sprite);
      }

      [DebuggerHidden]
      private IEnumerator HandleSubSpriteFall(tk2dSprite targetSprite, float adjustedStartHeight)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HangingObjectController__HandleSubSpriteFallc__Iterator0()
        {
          targetSprite = targetSprite,
          adjustedStartHeight = adjustedStartHeight,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator Fall()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HangingObjectController__Fallc__Iterator1()
        {
          _this = this
        };
      }

      private void HandleSubspritesLaunch()
      {
        if ((bool) (UnityEngine.Object) this.EnableRigidbodyPostFall)
          this.EnableRigidbodyPostFall.enabled = false;
        for (int index = 0; index < this.subSprites.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) this.subSprites[index])
          {
            MinorBreakable component = this.subSprites[index].GetComponent<MinorBreakable>();
            if ((bool) (UnityEngine.Object) component)
              component.enabled = true;
            DebrisObject orAddComponent = this.subSprites[index].gameObject.GetOrAddComponent<DebrisObject>();
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_boulder_break_01", this.gameObject);
            orAddComponent.angularVelocity = 45f;
            orAddComponent.angularVelocityVariance = 20f;
            orAddComponent.decayOnBounce = 0.5f;
            orAddComponent.bounceCount = 1;
            orAddComponent.canRotate = true;
            orAddComponent.shouldUseSRBMotion = true;
            orAddComponent.AssignFinalWorldDepth(-0.5f);
            orAddComponent.sprite = (tk2dBaseSprite) this.subSprites[index];
            orAddComponent.Trigger((this.subSprites[index].WorldCenter - this.objectSprite.WorldCenter).ToVector3ZUp(0.5f) * (float) UnityEngine.Random.Range(1, 2), (float) UnityEngine.Random.Range(1, 2));
          }
        }
      }

      public void Interact(PlayerController interactor) => this.TriggerFallInteracted();

      protected void TriggerFallInteracted()
      {
        if (this.m_hasFallen)
          return;
        this.m_hasFallen = true;
        if ((bool) (UnityEngine.Object) this.TriggerObjectBreakable)
        {
          if ((bool) (UnityEngine.Object) this.TriggerObjectBreakable.specRigidbody)
            this.TriggerObjectBreakable.specRigidbody.enabled = false;
          this.TriggerObjectBreakable.CleanupCallbacks();
          UnityEngine.Object.Destroy((UnityEngine.Object) this.TriggerObjectBreakable);
        }
        this.m_room.DeregisterInteractable((IPlayerInteractable) this);
        this.StartCoroutine(this.Fall());
      }

      protected void TriggerFallBroken()
      {
        if (this.m_hasFallen)
          return;
        this.m_hasFallen = true;
        this.m_room.DeregisterInteractable((IPlayerInteractable) this);
        this.StartCoroutine(this.Fall());
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
