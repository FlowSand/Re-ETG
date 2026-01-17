// Decompiled with JetBrains decompiler
// Type: TalkDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class TalkDoer : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      public List<TalkModule> modules;
      public Transform speakPoint;
      public string audioCharacterSpeechTag = string.Empty;
      [Header("Interactable Region")]
      public bool usesOverrideInteractionRegion;
      [ShowInInspectorIf("usesOverrideInteractionRegion", false)]
      public Vector2 overrideRegionOffset = Vector2.zero;
      [ShowInInspectorIf("usesOverrideInteractionRegion", false)]
      public Vector2 overrideRegionDimensions = Vector2.zero;
      [Header("Core Speech")]
      public string FirstMeetingEverModule = string.Empty;
      public string FirstMeetingSessionModule = string.Empty;
      public string RepeatMeetingSessionModule = string.Empty;
      [Header("Other Options")]
      [CheckAnimation(null)]
      public string fallbackAnimName = "idle";
      [CheckAnimation(null)]
      public string defaultSpeechAnimName = "talk";
      public bool usesCustomBetrayalLogic;
      public string betrayalSpeechKey = string.Empty;
      public bool betrayalSpeechSequential;
      private int betrayalSpeechIndex = -1;
      public string hitAnimName;
      public bool DoesVanish = true;
      public string vanishAnimName = "exit";
      public System.Action OnBetrayalWarning;
      public System.Action OnBetrayal;
      public List<GameObject> itemsToLeaveBehind;
      public bool alwaysWaitsForInput;
      public TalkDoer echo1;
      public TalkDoer echo2;
      public float conversationBreakRadius = 5f;
      public List<CharacterTalkModuleOverride> characterOverrides;
      public bool OverrideNoninteractable;
      protected TalkModule defaultModule;
      protected bool m_isTalking;
      protected bool m_uninteractable;
      protected int numTimesSpokenTo;
      protected int hitCount;
      protected bool m_isDealingWithBetrayal;
      private bool m_hack_isOpeningTruthChest;
      private bool m_isDoingForcedSpeech;
      protected PlayerController talkingPlayer;

      private void Start()
      {
        EncounterTrackable component = this.GetComponent<EncounterTrackable>();
        this.defaultModule = string.IsNullOrEmpty(this.FirstMeetingEverModule) || !((UnityEngine.Object) component != (UnityEngine.Object) null) || GameStatsManager.Instance.QueryEncounterable(component) != 0 ? (string.IsNullOrEmpty(this.FirstMeetingSessionModule) ? (string.IsNullOrEmpty(this.RepeatMeetingSessionModule) ? this.GetModuleFromName("start") : this.GetModuleFromName(this.RepeatMeetingSessionModule)) : this.GetModuleFromName(this.FirstMeetingSessionModule)) : this.GetModuleFromName(this.FirstMeetingEverModule);
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
          this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
        if (this.name.Contains("Truth_Knower"))
        {
          List<Chest> componentsInRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).GetComponentsInRoom<Chest>();
          for (int index = 0; index < componentsInRoom.Count; ++index)
          {
            if (componentsInRoom[index].name.ToLowerInvariant().Contains("truth"))
              componentsInRoom[index].majorBreakable.OnBreak += new System.Action(this.OnTruthChestBroken);
          }
        }
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
      }

      private void Update()
      {
        if (!this.m_isTalking || (double) Vector2.Distance(this.talkingPlayer.sprite.WorldCenter, this.sprite.WorldCenter) <= (double) this.conversationBreakRadius)
          return;
        this.ForceEndConversation();
      }

      private void OnRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (this.m_isTalking)
          return;
        Projectile component = rigidbodyCollision.OtherRigidbody.GetComponent<Projectile>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !(component.Owner is PlayerController) || this.m_isDealingWithBetrayal)
          return;
        ++this.hitCount;
        if (this.usesCustomBetrayalLogic)
        {
          if (this.hitCount < 2)
          {
            if (this.OnBetrayalWarning == null)
              return;
            this.OnBetrayalWarning();
          }
          else
          {
            if (this.hitCount >= 3 || this.OnBetrayal == null)
              return;
            this.OnBetrayal();
          }
        }
        else
        {
          if (!string.IsNullOrEmpty(this.hitAnimName))
            this.spriteAnimator.PlayForDuration(this.hitAnimName, -1f, this.fallbackAnimName);
          if (!this.DoesVanish || this.hitCount < 2)
          {
            if (string.IsNullOrEmpty(this.betrayalSpeechKey))
              return;
            this.StartCoroutine(this.HandleBetrayal());
          }
          else
            this.Vanish();
        }
      }

      private void OnTruthChestBroken()
      {
        this.StartCoroutine(this.HandleBetrayal());
        this.StartCoroutine(this.DelayedVanish(2f));
      }

      [DebuggerHidden]
      private IEnumerator DelayedVanish(float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<DelayedVanish>c__Iterator0()
        {
          delay = delay,
          $this = this
        };
      }

      private void Vanish()
      {
        GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).DeregisterInteractable((IPlayerInteractable) this);
        TextBoxManager.ClearTextBox(this.speakPoint);
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
          this.specRigidbody.enabled = false;
        tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.vanishAnimName);
        for (int index = 0; index < this.itemsToLeaveBehind.Count; ++index)
          this.itemsToLeaveBehind[index].transform.parent = this.transform.parent;
        if (clipByName != null)
          this.spriteAnimator.PlayAndDestroyObject(this.vanishAnimName);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      [DebuggerHidden]
      private IEnumerator HandleBetrayal()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<HandleBetrayal>c__Iterator1()
        {
          $this = this
        };
      }

      private TalkModule GetModuleFromName(string ID)
      {
        for (int index = 0; index < this.modules.Count; ++index)
        {
          if (this.modules[index].moduleID == ID)
            return this.modules[index];
        }
        return (TalkModule) null;
      }

      private void ProcessResponseAction(TalkResult result)
      {
        switch (result.action)
        {
          case TalkResult.TalkResultAction.CHANGE_DEFAULT_MODULE:
            this.defaultModule = this.GetModuleFromName(result.actionData);
            break;
          case TalkResult.TalkResultAction.OPEN_TRUTH_CHEST:
            this.StartCoroutine(this.DelayedChestOpen(3f));
            break;
          case TalkResult.TalkResultAction.VANISH:
            this.StartCoroutine(this.DelayedVanish(3f));
            break;
          case TalkResult.TalkResultAction.TOSS_CURRENT_GUN_IN_POT:
            WitchCauldronController component = this.transform.parent.GetComponent<WitchCauldronController>();
            if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
              break;
            component.TossPlayerEquippedGun(this.talkingPlayer);
            break;
          case TalkResult.TalkResultAction.RENDER_SILENT:
            this.StartCoroutine(this.MakeUninteractable(float.Parse(result.actionData)));
            break;
          case TalkResult.TalkResultAction.CHANGE_DEFAULT_MODULE_OF_OTHER_TALKDOER:
            result.objectData.GetComponent<TalkDoer>().defaultModule = result.objectData.GetComponent<TalkDoer>().GetModuleFromName(result.actionData);
            break;
          case TalkResult.TalkResultAction.SPAWN_ITEM:
            LootEngine.SpewLoot(result.objectData, (Vector3) (!((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null) ? this.sprite.WorldCenter : this.specRigidbody.UnitCenter));
            break;
          case TalkResult.TalkResultAction.MAKE_TALKDOER_INTERACTABLE:
            result.objectData.GetComponent<TalkDoer>().OverrideNoninteractable = false;
            break;
          case TalkResult.TalkResultAction.SPAWN_ITEM_FROM_TABLE:
            LootEngine.SpewLoot(result.lootTableData.SelectByWeightWithoutDuplicatesFullPrereqs((List<GameObject>) null), (Vector3) (!((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null) ? this.sprite.WorldCenter : this.specRigidbody.UnitCenter));
            break;
          case TalkResult.TalkResultAction.CUSTOM_ACTION:
            this.ProcessCustomAction(result.customActionID, result.actionData, result.objectData);
            break;
        }
      }

      [DebuggerHidden]
      private IEnumerator MakeUninteractable(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<MakeUninteractable>c__Iterator2()
        {
          duration = duration,
          $this = this
        };
      }

      private void OpenTruthChest()
      {
        List<Chest> componentsInRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).GetComponentsInRoom<Chest>();
        for (int index = 0; index < componentsInRoom.Count; ++index)
        {
          if (componentsInRoom[index].name.ToLowerInvariant().Contains("truth"))
          {
            componentsInRoom[index].IsLocked = false;
            componentsInRoom[index].IsSealed = false;
            tk2dSpriteAnimator componentInChildren = componentsInRoom[index].transform.Find("lock").GetComponentInChildren<tk2dSpriteAnimator>();
            if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
            {
              componentInChildren.StopAndResetFrame();
              this.StartCoroutine(this.PlayDelayedTruthChestLockOpen(componentInChildren, 1f));
            }
          }
        }
      }

      [DebuggerHidden]
      private IEnumerator DelayedChestOpen(float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<DelayedChestOpen>c__Iterator3()
        {
          delay = delay,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator PlayDelayedTruthChestLockOpen(tk2dSpriteAnimator lockAnimator, float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<PlayDelayedTruthChestLockOpen>c__Iterator4()
        {
          delay = delay,
          lockAnimator = lockAnimator
        };
      }

      private void ProcessCustomAction(string customActionID, string actionData, GameObject objectData)
      {
        UnityEngine.Debug.LogError((object) $"Custom action: {customActionID} is not implemented!");
      }

      private void BeginConversation(PlayerController player)
      {
        this.m_isTalking = true;
        GameUIRoot.Instance.InitializeConversationPortrait(player);
        EncounterTrackable component = this.GetComponent<EncounterTrackable>();
        if (this.numTimesSpokenTo == 0 && (UnityEngine.Object) component != (UnityEngine.Object) null)
          GameStatsManager.Instance.HandleEncounteredObject(component);
        ++this.numTimesSpokenTo;
        this.StartCoroutine(this.HandleConversationModule(this.defaultModule));
        if (!(this.defaultModule.moduleID == this.FirstMeetingSessionModule) && !(this.defaultModule.moduleID == this.FirstMeetingEverModule) || string.IsNullOrEmpty(this.RepeatMeetingSessionModule))
          return;
        this.defaultModule = this.GetModuleFromName(this.RepeatMeetingSessionModule);
      }

      private void ForceEndConversation()
      {
        TextBoxManager.ClearTextBox(this.speakPoint);
        this.StopAllCoroutines();
        if (this.m_hack_isOpeningTruthChest)
        {
          this.OpenTruthChest();
          this.m_hack_isOpeningTruthChest = false;
        }
        this.EndConversation();
      }

      private void EndConversation()
      {
        this.m_isTalking = false;
        if (string.IsNullOrEmpty(this.fallbackAnimName))
          return;
        this.spriteAnimator.Play(this.fallbackAnimName);
      }

      public void ForceTimedSpeech(
        string words,
        float initialDelay,
        float duration,
        TextBoxManager.BoxSlideOrientation slideOrientation)
      {
        UnityEngine.Debug.Log((object) ("starting forced timed speech: " + words));
        this.StartCoroutine(this.HandleForcedTimedSpeech(words, initialDelay, duration, slideOrientation));
      }

      [DebuggerHidden]
      private IEnumerator HandleForcedTimedSpeech(
        string words,
        float initialDelay,
        float duration,
        TextBoxManager.BoxSlideOrientation slideOrientation)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<HandleForcedTimedSpeech>c__Iterator5()
        {
          initialDelay = initialDelay,
          words = words,
          slideOrientation = slideOrientation,
          duration = duration,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleConversationModule(TalkModule module)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<HandleConversationModule>c__Iterator6()
        {
          module = module,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleResponses(
        TalkModule module,
        string overrideResponse1,
        string overrideResponse2,
        string overrideFollowupModule1,
        string overrideFollowupModule2)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<HandleResponses>c__Iterator7()
        {
          module = module,
          overrideResponse1 = overrideResponse1,
          overrideResponse2 = overrideResponse2,
          overrideFollowupModule1 = overrideFollowupModule1,
          overrideFollowupModule2 = overrideFollowupModule2,
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator WaitForTextRevealed()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<WaitForTextRevealed>c__Iterator8()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator WaitForPlayer()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoer.<WaitForPlayer>c__Iterator9()
        {
          $this = this
        };
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if (!(bool) (UnityEngine.Object) this || this.m_uninteractable || this.OverrideNoninteractable)
          return 1000f;
        if (this.usesOverrideInteractionRegion)
          return BraveMathCollege.DistToRectangle(point, this.transform.position.XY() + this.overrideRegionOffset * (1f / 16f), this.overrideRegionDimensions * (1f / 16f));
        Bounds bounds = this.sprite.GetBounds();
        return BraveMathCollege.DistToRectangle(point, (Vector2) (this.sprite.transform.position + bounds.center - bounds.extents), (Vector2) bounds.size);
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if (this.m_isTalking || !(bool) (UnityEngine.Object) this)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        this.sprite.UpdateZDepth();
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (this.m_isTalking || !(bool) (UnityEngine.Object) this)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
        this.sprite.UpdateZDepth();
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_isTalking)
          return;
        this.talkingPlayer = interactor;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
        this.BeginConversation(interactor);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
