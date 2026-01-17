// Decompiled with JetBrains decompiler
// Type: PastLabMarineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class PastLabMarineController : MonoBehaviour
    {
      public tk2dSpriteAnimator LeftRedMarine;
      public tk2dSpriteAnimator LeftGreenMarine;
      public tk2dSpriteAnimator RightRedMarine;
      public tk2dSpriteAnimator RightGreenMarine;
      public Transform LeftRedMarineShootPoint;
      public Transform RightRedMarineShootPoint;
      public DungeonDoorController AreaDoor;
      public DungeonDoorController CellDoor;
      public Transform[] SpeakPoints;
      public TalkDoerLite VictoryTalkDoer;
      public SpeculativeRigidbody[] BossCollisionExceptions;
      public ScreenShakeSettings AmbientScreenShakeSettings;
      public float MinTimeBetweenAmbientShakes = 3f;
      public float MaxTimeBetweenAmbientShakes = 5f;
      public tk2dSpriteAnimator TerrorPortal;
      private bool m_inCombat;
      private bool m_occupied;
      private int m_idleCounter;
      private AIBulletBank m_bulletBank;
      private bool m_hasRemarkedOnDoorway;
      private bool m_forceSkip;

      public void Engage()
      {
        this.m_bulletBank = this.GetComponent<AIBulletBank>();
        this.m_inCombat = true;
        this.AreaDoor.DoSeal(GameManager.Instance.Dungeon.data.Entrance);
        this.StartCoroutine(this.HandlePortal());
        HealthHaver healthHaver = StaticReferenceManager.AllHealthHavers.Find((Predicate<HealthHaver>) (h => h.IsBoss));
        healthHaver.GetComponent<ObjectVisibilityManager>().ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT);
        healthHaver.GetComponent<GenericIntroDoer>().TriggerSequence(GameManager.Instance.PrimaryPlayer);
        for (int index = 0; index < this.BossCollisionExceptions.Length; ++index)
          healthHaver.specRigidbody.RegisterSpecificCollisionException(this.BossCollisionExceptions[index]);
        this.StartCoroutine(this.HandleDialogue("#PRIMERDYNE_MARINE_ENTRY_01"));
      }

      [DebuggerHidden]
      private IEnumerator HandlePortal()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandlePortal>c__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<Start>c__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleInitialRoomLockdown()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandleInitialRoomLockdown>c__Iterator2()
        {
          _this = this
        };
      }

      private void Update()
      {
        this.m_forceSkip = false;
        bool flag = BraveInput.GetInstanceForPlayer(0).WasAdvanceDialoguePressed();
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          flag |= BraveInput.GetInstanceForPlayer(1).WasAdvanceDialoguePressed();
        if (flag)
          this.m_forceSkip = true;
        if (this.m_inCombat && !this.m_occupied)
        {
          if ((double) UnityEngine.Random.value > 0.5 && this.m_idleCounter < 2)
            this.StartCoroutine(this.HandleHide());
          else
            this.StartCoroutine(this.HandleShoot());
        }
        if (this.m_hasRemarkedOnDoorway || (double) GameManager.Instance.PrimaryPlayer.transform.position.x <= 70.0)
          return;
        this.m_hasRemarkedOnDoorway = true;
        this.MakeSoldierTalkAmbient("#PRIMERDYNE_MARINE_CANT_LEAVE", 5f, true);
      }

      [DebuggerHidden]
      private IEnumerator HandleHide()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandleHide>c__Iterator3()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleShoot()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandleShoot>c__Iterator4()
        {
          _this = this
        };
      }

      private void FireBullet(Transform shootPoint, Vector2 dirVec)
      {
        this.m_bulletBank.CreateProjectileFromBank((Vector2) shootPoint.position, BraveMathCollege.Atan2Degrees(dirVec.normalized) + UnityEngine.Random.Range(-10f, 10f), "default").GetComponent<Projectile>().collidesWithPlayer = false;
      }

      public void MakeSoldierTalkAmbient(string stringKey, float duration = 3f, bool isThoughtBubble = false)
      {
        this.DoAmbientTalk(GameManager.Instance.PrimaryPlayer.transform, new Vector3(0.75f, 1.5f, 0.0f), stringKey, duration, isThoughtBubble);
      }

      public void DoAmbientTalk(
        Transform baseTransform,
        Vector3 offset,
        string stringKey,
        float duration,
        bool isThoughtBubble = false)
      {
        if (isThoughtBubble)
          TextBoxManager.ShowThoughtBubble(baseTransform.position + offset, baseTransform, -1f, StringTableManager.GetString(stringKey), false, overrideAudioTag: GameManager.Instance.PrimaryPlayer.characterAudioSpeechTag);
        else
          TextBoxManager.ShowTextBox(baseTransform.position + offset, baseTransform, -1f, StringTableManager.GetString(stringKey), GameManager.Instance.PrimaryPlayer.characterAudioSpeechTag, false);
        this.StartCoroutine(this.HandleManualTalkDuration(baseTransform, duration));
      }

      [DebuggerHidden]
      private IEnumerator HandleManualTalkDuration(Transform source, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandleManualTalkDuration>c__Iterator5()
        {
          duration = duration,
          source = source,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleDialogue(string stringKey)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandleDialogue>c__Iterator6()
        {
          stringKey = stringKey,
          _this = this
        };
      }

      public void OnBossKilled()
      {
        this.m_inCombat = false;
        GameStatsManager.Instance.SetCharacterSpecificFlag(PlayableCharacters.Soldier, CharacterSpecificGungeonFlags.KILLED_PAST, true);
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_KILLED_PAST, 1f);
        GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_SOLDIER_PAST, true);
        this.StartCoroutine(this.HandleBossKilled());
      }

      [DebuggerHidden]
      private IEnumerator HandleBossKilled()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PastLabMarineController.<HandleBossKilled>c__Iterator7()
        {
          _this = this
        };
      }
    }

}
